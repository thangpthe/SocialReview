/**
 * product-detail.js  — Trustify
 * Handles: Review submit (AJAX), Comment toggle + submit (AJAX), Reaction toggle (AJAX)
 *
 * BUG FIX: antiForgeryToken was read from logout form — now reads from the review form itself
 * BUG FIX: isAuthenticated detection via data attribute on body
 */

document.addEventListener('DOMContentLoaded', function () {

    // ── TOKEN & AUTH ──────────────────────────────────────────────────────────
    // Read token from the review form (most reliable location)
    const tokenEl =
        document.querySelector('#form-create-review input[name="__RequestVerificationToken"]') ||
        document.querySelector('input[name="__RequestVerificationToken"]');
    const antiForgeryToken = tokenEl ? tokenEl.value : null;

    // Detect authentication: check if profile link exists (user icon in header)
    const isAuthenticated = !!document.querySelector('.user-menu-link');

    function requireAuth() {
        if (!isAuthenticated) {
            const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
            window.location.href = '/Auth/Login?ReturnUrl=' + returnUrl;
            return false;
        }
        if (!antiForgeryToken) {
            console.warn('AntiForgeryToken not found. AJAX may fail.');
        }
        return true;
    }

    function showToast(type, message) {
        const existing = document.getElementById('page-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.id = 'page-toast';
        toast.setAttribute('role', 'alert');
        toast.style.cssText = `
            position: fixed; bottom: 1.5rem; right: 1.5rem; z-index: 9999;
            background: ${type === 'success' ? '#065f46' : '#991b1b'};
            color: #fff; padding: .85rem 1.35rem;
            border-radius: 12px; font-weight: 600; font-size: .875rem;
            box-shadow: 0 8px 24px rgba(0,0,0,.2);
            display: flex; align-items: center; gap: .5rem;
            animation: slideUp .3s ease;
        `;

        const style = document.createElement('style');
        style.textContent = '@keyframes slideUp{from{opacity:0;transform:translateY(16px)}to{opacity:1;transform:translateY(0)}}';
        if (!document.getElementById('toast-style')) {
            style.id = 'toast-style';
            document.head.appendChild(style);
        }

        toast.innerHTML = `<i class="fa-solid fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i> ${message}`;
        document.body.appendChild(toast);
        setTimeout(() => toast.remove(), 4000);
    }

    // ── 1. REVIEW FORM SUBMIT ─────────────────────────────────────────────────
    const reviewForm = document.getElementById('form-create-review');
    const reviewErrorBox = document.getElementById('review-form-error');
    const submitBtn = document.getElementById('btn-submit-review');

    if (reviewForm) {
        reviewForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            if (!requireAuth()) return;

            const originalText = submitBtn.innerHTML;
            submitBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Đang gửi...';
            submitBtn.disabled = true;
            if (reviewErrorBox) reviewErrorBox.classList.add('hidden');

            try {
                const resp = await fetch(reviewForm.action, {
                    method: 'POST',
                    body: new FormData(reviewForm),
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });

                if (resp.ok) {
                    const html = await resp.text();
                    const reviewsList = document.querySelector('.reviews-list');
                    if (reviewsList) {
                        // Remove empty placeholder if present
                        reviewsList.querySelector('.reviews-empty')?.remove();
                        reviewsList.insertAdjacentHTML('afterbegin', html);

                        // Scroll to new review
                        const firstReview = reviewsList.firstElementChild;
                        if (firstReview) {
                            firstReview.scrollIntoView({ behavior: 'smooth', block: 'start' });
                            firstReview.style.animation = 'highlightNew .8s ease';
                        }
                    }
                    reviewForm.reset();
                    showToast('success', 'Đánh giá đã được gửi thành công!');

                    // Add highlight CSS once
                    if (!document.getElementById('highlight-style')) {
                        const s = document.createElement('style');
                        s.id = 'highlight-style';
                        s.textContent = '@keyframes highlightNew{0%{box-shadow:0 0 0 4px #7c3aed40}100%{box-shadow:none}}';
                        document.head.appendChild(s);
                    }
                } else {
                    const data = await resp.json().catch(() => ({}));
                    const msg = data.errors ? data.errors.join(', ') : 'Vui lòng điền đầy đủ thông tin.';
                    if (reviewErrorBox) {
                        reviewErrorBox.textContent = msg;
                        reviewErrorBox.classList.remove('hidden');
                    }
                    showToast('error', 'Không thể gửi đánh giá.');
                }
            } catch (err) {
                console.error('Review submit error:', err);
                showToast('error', 'Lỗi kết nối. Vui lòng thử lại.');
            } finally {
                submitBtn.innerHTML = originalText;
                submitBtn.disabled = false;
            }
        });
    }

    // ── 2. COMMENT TOGGLE & SUBMIT ────────────────────────────────────────────
    // Use event delegation on the reviews section
    const reviewsSection = document.getElementById('reviews');

    if (reviewsSection) {
        reviewsSection.addEventListener('click', function (e) {
            // Toggle comment panel
            const toggleBtn = e.target.closest('.btn-toggle-comment');
            if (toggleBtn) {
                e.preventDefault();
                const reviewId = toggleBtn.dataset.reviewId;
                const section = document.getElementById('comment-section-' + reviewId);
                if (section) {
                    const isHidden = section.classList.contains('hidden');
                    section.classList.toggle('hidden');
                    toggleBtn.setAttribute('aria-expanded', isHidden ? 'true' : 'false');

                    if (isHidden) {
                        // Focus comment input
                        const input = section.querySelector('input[name="Content"]');
                        if (input) setTimeout(() => input.focus(), 100);
                    }
                }
                return;
            }

            // Submit comment
            const commentSubmit = e.target.closest('.btn-submit-comment');
            if (commentSubmit) {
                e.preventDefault();
                if (!requireAuth()) return;
                const form = commentSubmit.closest('.comment-form');
                if (form) submitComment(form);
                return;
            }

            // Reaction
            const reactionBtn = e.target.closest('.btn-toggle-reaction');
            if (reactionBtn) {
                e.preventDefault();
                if (!requireAuth()) return;
                handleReaction(reactionBtn);
            }
        });

        // Also handle form submit event (Enter key in comment input)
        reviewsSection.addEventListener('submit', function (e) {
            if (e.target.classList.contains('comment-form')) {
                e.preventDefault();
                if (!requireAuth()) return;
                submitComment(e.target);
            }
        });
    }

    async function submitComment(form) {
        const reviewId = form.dataset.reviewId;
        const errorDiv = document.getElementById('comment-error-' + reviewId);
        const commentList = document.getElementById('comment-list-' + reviewId);
        const input = form.querySelector('input[name="Content"]');
        const submitBtn = form.querySelector('.btn-submit-comment');

        if (!input || !input.value.trim()) {
            if (errorDiv) {
                errorDiv.textContent = 'Vui lòng nhập nội dung bình luận.';
                errorDiv.classList.remove('hidden');
            }
            input?.focus();
            return;
        }

        if (errorDiv) errorDiv.classList.add('hidden');
        if (submitBtn) submitBtn.disabled = true;

        try {
            const resp = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (resp.ok) {
                const html = await resp.text();
                if (commentList) {
                    // Remove "no comments" placeholder
                    commentList.querySelector('.no-comments')?.remove();
                    commentList.insertAdjacentHTML('beforeend', html);
                }
                input.value = '';

                // Update comment count on toggle button
                const toggleBtn = document.querySelector(`.btn-toggle-comment[data-review-id="${reviewId}"] span`);
                if (toggleBtn) {
                    const match = toggleBtn.textContent.match(/\d+/);
                    const current = match ? parseInt(match[0]) : 0;
                    toggleBtn.textContent = '(' + (current + 1) + ')';
                }
            } else {
                const data = await resp.json().catch(() => ({}));
                if (errorDiv) {
                    errorDiv.textContent = data.errors ? data.errors.join(', ') : 'Không thể gửi bình luận.';
                    errorDiv.classList.remove('hidden');
                }
            }
        } catch (err) {
            console.error('Comment submit error:', err);
            if (errorDiv) {
                errorDiv.textContent = 'Lỗi kết nối. Vui lòng thử lại.';
                errorDiv.classList.remove('hidden');
            }
        } finally {
            if (submitBtn) submitBtn.disabled = false;
        }
    }

    // ── 3. REACTION TOGGLE ────────────────────────────────────────────────────
    async function handleReaction(btn) {
        const reviewId = btn.dataset.reviewId;
        const reactionType = btn.dataset.type || 'Helpful';

        btn.disabled = true;

        const formData = new FormData();
        formData.append('reviewId', reviewId);
        formData.append('reactionType', reactionType);

        try {
            const resp = await fetch('/api/reaction/toggle', {
                method: 'POST',
                body: formData,
                headers: { 'RequestVerificationToken': antiForgeryToken || '' }
            });

            if (resp.ok) {
                const data = await resp.json();
                const countSpan = btn.querySelector('span');
                if (countSpan) countSpan.textContent = '(' + data.newCount + ')';

                if (data.userHasReacted) {
                    btn.classList.add('text-purple-600');
                    btn.style.color = '#7c3aed';
                    btn.style.fontWeight = '700';
                } else {
                    btn.classList.remove('text-purple-600');
                    btn.style.color = '';
                    btn.style.fontWeight = '';
                }
            } else if (resp.status === 401) {
                requireAuth(); // redirect to login
            } else {
                showToast('error', 'Không thể thực hiện hành động này.');
            }
        } catch (err) {
            console.error('Reaction error:', err);
            showToast('error', 'Lỗi kết nối.');
        } finally {
            btn.disabled = false;
        }
    }

});