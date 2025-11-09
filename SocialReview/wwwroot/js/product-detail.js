// Biến global để lấy Token (nếu người dùng đã đăng nhập)
const antiForgeryTokenEl = document.querySelector('form[action="/Auth/Logout"] input[name="__RequestVerificationToken"]');
const antiForgeryToken = antiForgeryTokenEl ? antiForgeryTokenEl.value : null;

// Biến global để kiểm tra trạng thái đăng nhập
const isAuthenticated = document.body.dataset.isAuthenticated === 'true';

// --- HÀM 1: KIỂM TRA BẢO VỆ CHUNG ---
function checkAuthentication() {
    // ... (Giữ nguyên code)
    if (!isAuthenticated) {
        window.location.href = '/Auth/Login?ReturnUrl=' + encodeURIComponent(window.location.pathname);
        return false;
    }
    if (!antiForgeryToken) {
        console.error("LỖI BẢO MẬT: Không tìm thấy __RequestVerificationToken.");
        alert("Lỗi bảo mật, không thể thực hiện. Vui lòng tải lại trang.");
        return false;
    }
    return true;
}

// --- HÀM 2: HIỂN THỊ LỖI ---
function displayError(errorElement, technicalError, userMessage) {
    // ... (Giữ nguyên code)
    if (errorElement) {
        console.error(technicalError); // Log lỗi kỹ thuật
        errorElement.textContent = userMessage; // Hiển thị lỗi thân thiện
        errorElement.classList.remove('hidden');
    } else {
        console.error(technicalError);
        alert(userMessage); // Fallback
    }
}

// --- HÀM 3: GỬI REVIEW MỚI ---
async function handleReviewSubmit(e) {
    // ... (Giữ nguyên code)
    e.preventDefault();
    if (!checkAuthentication()) return;

    const reviewForm = e.target;
    const reviewFormError = document.getElementById('review-form-error');
    const submitReviewButton = document.getElementById('btn-submit-review');

    if (!submitReviewButton || !reviewFormError) return;

    const originalButtonText = submitReviewButton.innerHTML;
    submitReviewButton.innerHTML = "Đang gửi...";
    submitReviewButton.disabled = true;
    reviewFormError.classList.add('hidden');

    try {
        const response = await fetch(reviewForm.action, {
            method: 'POST',
            body: new FormData(reviewForm),
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => {
                throw new Error("Lỗi máy chủ, không thể gửi đánh giá.");
            });
            throw new Error(errorData.errors ? errorData.errors.join(', ') : "Lỗi không xác định.");
        }

        const html = await response.text();
        const reviewsList = document.querySelector('.reviews-list');

        if (reviewsList) {
            const noReviewsMsg = reviewsList.querySelector('.card[style*="text-align: center"]');
            if (noReviewsMsg) {
                noReviewsMsg.remove();
            }

            reviewsList.insertAdjacentHTML('afterbegin', html);
            reviewForm.reset(); // Xóa form

            // (MỚI) Cập nhật số đếm review
            const reviewCountEl = document.querySelector('#reviews .section-title');
            if (reviewCountEl) {
                // Trích xuất số đếm cũ và + 1
                const countMatch = reviewCountEl.textContent.match(/\((\d+)\)/);
                let currentCount = countMatch ? parseInt(countMatch[1]) : 0;
                currentCount++;
                reviewCountEl.textContent = reviewCountEl.textContent.replace(/\(\d+\)/, `(${currentCount})`);
            }
            // (MỚI) Xóa ảnh preview
            document.getElementById('image-preview-container').innerHTML = '';
        }

    } catch (error) {
        displayError(reviewFormError, `Lỗi AJAX (Viết Review): ${error.message}`, "Đã xảy ra lỗi. Vui lòng thử lại sau.");
    } finally {
        submitReviewButton.innerHTML = originalButtonText;
        submitReviewButton.disabled = false;
    }
}

// --- HÀM 4: GỬI COMMENT MỚI ---
async function handleCommentSubmit(commentForm) {
    // ... (Giữ nguyên code)
    if (!checkAuthentication()) return;

    const reviewId = commentForm.dataset.reviewId;
    const errorDiv = document.getElementById(`comment-error-${reviewId}`);
    const commentList = document.getElementById(`comment-list-${reviewId}`);
    const commentInput = commentForm.querySelector('input[name="Content"]');

    if (!errorDiv || !commentList || !commentInput) return;

    if (!commentInput.value.trim()) {
        displayError(errorDiv, "Comment input is empty.", "Vui lòng nhập nội dung bình luận.");
        return;
    }

    errorDiv.classList.add('hidden');
    const submitButton = commentForm.querySelector('.btn-submit-comment');
    if (submitButton) submitButton.disabled = true;

    try {
        const response = await fetch(commentForm.action, {
            method: 'POST',
            body: new FormData(commentForm),
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) {
            const errorData = await response.json().catch(() => {
                throw new Error("Lỗi máy chủ, không thể gửi bình luận.");
            });
            throw new Error(errorData.errors.join(', '));
        }

        const html = await response.text();

        // Xóa "Chưa có bình luận" (nếu có)
        const noCommentsMsg = commentList.querySelector('.no-comments');
        if (noCommentsMsg) {
            noCommentsMsg.remove();
        }

        commentList.insertAdjacentHTML('beforeend', html);
        commentInput.value = ''; // Xóa input

        // (MỚI) Cập nhật số đếm bình luận
        const reviewWrapper = commentForm.closest('.review-card-wrapper');
        const commentButton = reviewWrapper.querySelector(`.btn-toggle-comment[data-review-id="${reviewId}"]`);
        const countSpan = commentButton.querySelector('span');

        let currentCount = parseInt(countSpan.textContent.replace(/[()]/g, '')) || 0;
        currentCount++;
        countSpan.textContent = `(${currentCount})`;

    } catch (error) {
        displayError(errorDiv, `Lỗi AJAX (Gửi Comment): ${error.message}`, "Đã xảy ra lỗi khi gửi bình luận.");
    } finally {
        if (submitButton) submitButton.disabled = false;
    }
}

// --- HÀM 5: REACTION CHO REVIEW (HỮU ÍCH / BÁO CÁO) ---
async function handleReviewReactionToggle(reactionButton) {
    // ... (Giữ nguyên code)
    if (!checkAuthentication()) return;

    const reviewId = reactionButton.dataset.reviewId;
    const reactionType = reactionButton.dataset.type || "Helpful";

    reactionButton.disabled = true;

    const formData = new FormData();
    formData.append('reviewId', reviewId);
    formData.append('reactionType', reactionType);

    try {
        const response = await fetch('/api/reaction/toggle', {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) {
            throw new Error('Lỗi máy chủ khi reaction.');
        }

        const data = await response.json();

        if (reactionType === 'Report') {
            alert('Đã báo cáo đánh giá. Cảm ơn bạn!');
            reactionButton.textContent = 'Đã báo cáo';
            return;
        }

        const countSpan = reactionButton.querySelector('span');
        if (countSpan) {
            countSpan.textContent = `(${data.newCount})`;
        }

        reactionButton.classList.toggle('active', data.userHasReacted);

    } catch (error) {
        console.error('Lỗi AJAX Reaction:', error.message);
        alert("Đã xảy ra lỗi: " + error.message);
    } finally {
        if (reactionType !== 'Report') {
            reactionButton.disabled = false;
        }
    }
}

// --- HÀM 6: REACTION CHO COMMENT (THÍCH / BÁO CÁO) ---
async function handleCommentReactionClick(button) {
    // ... (Giữ nguyên code)
    if (!checkAuthentication()) return;

    const commentId = button.dataset.commentId;
    const reactionType = button.dataset.type;

    button.disabled = true;

    const formData = new FormData();
    formData.append('commentId', commentId);
    formData.append('reactionType', reactionType);

    try {
        const response = await fetch('/api/CommentReaction/toggle', {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) throw new Error('Lỗi reaction.');
        const data = await response.json();

        if (reactionType === 'Like') {
            button.querySelector('.like-count').textContent = data.newCount;
            button.classList.toggle('reacted', data.userHasReacted);
        } else if (reactionType === 'Report') {
            alert('Đã báo cáo bình luận. Cảm ơn bạn!');
            button.textContent = 'Đã báo cáo';
        }
    } catch (err) {
        alert(err.message);
    } finally {
        if (reactionType !== 'Report') {
            button.disabled = false;
        }
    }
}

// --- HÀM 7: XỬ LÝ SỬA REVIEW (NỘI TUYẾN) ---
async function handleEditReviewClick(editButton) {
    // ... (Giữ nguyên code)
    const reviewCardWrapper = editButton.closest('.review-card-wrapper');
    const reviewCardContent = reviewCardWrapper.querySelector('.review-card-content');
    const reviewId = editButton.dataset.reviewId;

    if (reviewCardContent.dataset.isEditing === 'true') return;
    reviewCardContent.dataset.isEditing = 'true';

    editButton.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i>';

    try {
        const response = await fetch(`/Product/GetReviewDetails?id=${reviewId}`);
        if (!response.ok) throw new Error('Không thể lấy dữ liệu.');
        const data = await response.json();

        reviewCardContent.dataset.originalHtml = reviewCardContent.innerHTML;
        const formHtml = createEditFormHtml(data);
        reviewCardContent.innerHTML = formHtml;

        reviewCardContent.querySelector('.form-edit-inline')
            .addEventListener('submit', handleSaveReviewSubmit);

    } catch (err) {
        alert('Lỗi: ' + err.message);
        editButton.innerHTML = '<i class="fa-solid fa-pen-to-square"></i> Sửa';
        reviewCardContent.dataset.isEditing = 'false';
    }
}

// --- HÀM 8: LƯU REVIEW (SAU KHI SỬA) ---
async function handleSaveReviewSubmit(e) {
    // ... (Giữ nguyên code)
    e.preventDefault();
    const form = e.target;
    const reviewCardContent = form.closest('.review-card-content');
    const reviewId = form.querySelector('input[name="ReviewID"]').value;

    const submitButton = form.querySelector('.btn-save-inline');
    submitButton.disabled = true;
    submitButton.textContent = 'Đang lưu...';

    const formData = new FormData(form);
    const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const response = await fetch('/Product/UpdateReview', {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': token }
        });

        if (!response.ok) throw new Error('Lỗi khi cập nhật.');
        const data = await response.json();

        // Tạo HTML tĩnh MỚI (dựa trên data trả về và HTML cũ)
        const originalHtml = reviewCardContent.dataset.originalHtml;
        const newStaticHtml = createStaticReviewHtml(data, originalHtml);

        reviewCardContent.innerHTML = newStaticHtml;
        reviewCardContent.dataset.isEditing = 'false';
        delete reviewCardContent.dataset.originalHtml;

    } catch (err) {
        alert('Lỗi: ' + err.message);
        submitButton.disabled = false;
        submitButton.textContent = 'Lưu';
    }
}

// --- HÀM 9: HỦY SỬA REVIEW ---
function handleCancelReviewClick(cancelButton) {
    // ... (Giữ nguyên code)
    const reviewCardContent = cancelButton.closest('.review-card-content');
    const originalHtml = reviewCardContent.dataset.originalHtml;

    if (originalHtml) {
        reviewCardContent.innerHTML = originalHtml;
        reviewCardContent.dataset.isEditing = 'false';
        delete reviewCardContent.dataset.originalHtml;
    }
}

// --- HÀM 10: XỬ LÝ SỬA COMMENT (NỘI TUYẾN) ---
async function handleEditCommentClick(button) {
    // ... (Giữ nguyên code)
    const commentCard = button.closest('.comment-card');
    const commentBody = commentCard.querySelector('.comment-body');
    const commentId = button.dataset.commentId;

    if (commentBody.classList.contains('is-editing')) return; // Đã sửa
    commentBody.classList.add('is-editing');

    try {
        const response = await fetch(`/Comment/GetComment?id=${commentId}`);
        if (!response.ok) throw new Error('Không lấy được comment');
        const data = await response.json();

        // Lưu HTML gốc vào data-attribute
        commentBody.dataset.originalHtml = commentBody.innerHTML;

        commentBody.innerHTML = `
            <form class="form-edit-comment-inline">
                <textarea class="form-textarea-inline" required>${escapeHTML(data.content)}</textarea>
                <div class="comment-actions">
                    <button type="button" class="action-btn-xs btn-cancel-comment">Hủy</button>
                    <button type="submit" class="action-btn-xs btn-save-comment">Lưu</button>
                </div>
            </form>
        `;

        // Gắn listener cho form mới (submit)
        commentBody.querySelector('.form-edit-comment-inline').addEventListener('submit', async (e) => {
            e.preventDefault();
            await saveCommentEdit(commentId, commentBody);
        });

        // Gắn listener cho nút Hủy
        commentBody.querySelector('.btn-cancel-comment').addEventListener('click', () => {
            const originalHtml = commentBody.dataset.originalHtml;
            if (originalHtml) {
                commentBody.innerHTML = originalHtml;
                commentBody.classList.remove('is-editing');
            }
        });

    } catch (err) {
        alert(err.message);
        commentBody.classList.remove('is-editing');
    }
}

// --- HÀM 11: LƯU COMMENT (SAU KHI SỬA) ---
async function saveCommentEdit(commentId, commentBody) {
    // ... (Giữ nguyên code)
    const textarea = commentBody.querySelector('.form-textarea-inline');
    const content = textarea.value;

    const formData = new FormData();
    formData.append('CommentID', commentId);
    formData.append('Content', content);
    formData.append('__RequestVerificationToken', antiForgeryToken);

    try {
        const response = await fetch('/Comment/Update', {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) throw new Error('Lỗi khi lưu.');
        const data = await response.json();

        // Khôi phục HTML gốc và cập nhật nội dung
        const originalHtml = commentBody.dataset.originalHtml;
        const parser = new DOMParser();
        const doc = parser.parseFromString(originalHtml, 'text/html');
        doc.querySelector('.comment-content').textContent = data.newContent; // Cập nhật nội dung mới

        commentBody.innerHTML = doc.body.innerHTML; // Lấy lại nội dung
        commentBody.classList.remove('is-editing');

    } catch (err) {
        alert(err.message);
    }
}

// === THÊM HÀM 12 MỚI ===
// --- HÀM 12: XỬ LÝ XÓA COMMENT ---
async function handleDeleteCommentClick(button) {
    if (!checkAuthentication()) return;

    const commentId = button.dataset.commentId;

    //if (!confirm("Bạn có chắc chắn muốn xóa bình luận này không? Thao tác này không thể hoàn tác.")) {
    //    return;
    //}

    // === BẮT ĐẦU SỬA LỖI ===
    // 1. LẤY TẤT CẢ PHẦN TỬ CẦN THIẾT TRƯỚC
    const commentCard = button.closest('.comment-card');
    const reviewWrapper = button.closest('.review-card-wrapper');

    if (!commentCard || !reviewWrapper) {
        console.error('Lỗi DOM: Không tìm thấy thẻ cha khi xóa bình luận.');
        return;
    }

    const commentButton = reviewWrapper.querySelector('.btn-toggle-comment');
    const countSpan = commentButton ? commentButton.querySelector('span') : null;
    // === KẾT THÚC SỬA LỖI ===

    const formData = new FormData();
    formData.append('id', commentId);

    try {
        const response = await fetch('/Comment/Delete', {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': antiForgeryToken }
        });

        if (!response.ok) {
            throw new Error('Lỗi khi xóa bình luận.');
        }

        const data = await response.json();
        if (data.success) {
            // 2. Xóa khỏi giao diện
            commentCard.remove(); // <-- Bây giờ mới xóa

            // 3. Cập nhật (giảm) số đếm bình luận
            if (countSpan) {
                let currentCount = parseInt(countSpan.textContent.replace(/[()]/g, '')) || 0;
                currentCount = Math.max(0, currentCount - 1); // Tránh số âm
                countSpan.textContent = `(${currentCount})`;
            }
        }

    } catch (err) {
        console.error(err.message);
    }
}


// --- HÀM 13: CÁC HÀM TRỢ GIÚP (HELPERS) ---
// ... (Giữ nguyên code)
// Helper: Tạo HTML cho form Sửa Review
function createEditFormHtml(data) {
    return `
        <form class="form-edit-inline" data-review-id="${data.reviewID}">
            <input type="hidden" name="__RequestVerificationToken" value="${antiForgeryToken}" />
            <input type="hidden" name="ReviewID" value="${data.reviewID}" />
            
            <div class="form-group">
                <label class="form-label">Đánh giá *</label>
                <select name="Rating" class="form-select" required>
                    <option value="5" ${data.rating == 5 ? 'selected' : ''}>5 sao - Rất hài lòng</option>
                    <option value="4" ${data.rating == 4 ? 'selected' : ''}>4 sao - Hài lòng</option>
                    <option value="3" ${data.rating == 3 ? 'selected' : ''}>3 sao - Bình thường</option>
                    <option value="2" ${data.rating == 2 ? 'selected' : ''}>2 sao - Không hài lòng</option>
                    <option value="1" ${data.rating == 1 ? 'selected' : ''}>1 sao - Rất tệ</option>
                </select>
            </div>
            <div class="form-group">
                <label class="form-label">Tiêu đề *</label>
                <input name="Title" class="form-input" required value="${escapeHTML(data.title)}">
            </div>
            <div class="form-group">
                <label class="form-label">Nội dung *</label>
                <textarea name="Content" class="form-textarea" rows="4" required>${escapeHTML(data.content)}</textarea>
            </div>
            
            <div class="review-actions-edit">
                <button type="button" class="action-btn btn-cancel-inline">Hủy</button>
                <button type="submit" class="action-btn btn-save-inline">Lưu</button>
            </div>
        </form>
    `;
}

// Helper: Tạo lại HTML Tĩnh (sau khi lưu)
function createStaticReviewHtml(data, originalHtml) {
    const parser = new DOMParser();
    const oldDoc = parser.parseFromString(originalHtml, 'text/html');
    const headerHtml = oldDoc.querySelector('.review-header') ? oldDoc.querySelector('.review-header').outerHTML : '';
    const actionsHtml = oldDoc.querySelector('.review-actions') ? oldDoc.querySelector('.review-actions').outerHTML : '';
    const reviewImageHtml = oldDoc.querySelector('.review-image-gallery') ? oldDoc.querySelector('.review-image-gallery').outerHTML : '';

    let starsHtml = '';
    for (let i = 1; i <= 5; i++) {
        starsHtml += `<i class="${i <= data.newRating ? 'fa-solid fa-star' : 'fa-regular fa-star'}"></i>`;
    }

    return `
        ${headerHtml}
        <div class="star-rating" title="${data.newRating} sao" style="margin-bottom: 0.5rem;">
            ${starsHtml}
        </div>
        <h4>${escapeHTML(data.newTitle)}</h4>
        <p>${escapeHTML(data.newContent)}</p>
        ${reviewImageHtml}
        ${actionsHtml}
    `;
}

// Helper: Chống XSS (Cross-Site Scripting)
function escapeHTML(str) {
    if (!str) return '';
    return str.replace(/[&<>"']/g, function (m) {
        return {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        }[m];
    });
}


// --- HÀM 14: CÀI ĐẶT CÁC EVENT LISTENER CHÍNH ---
// (Đổi tên hàm 13 cũ thành 14)
function setupEventListeners() {

    // 1. Gắn Listener cho Form Viết Review (nếu nó tồn tại)
    const reviewForm = document.getElementById('form-create-review');
    if (reviewForm) {
        reviewForm.addEventListener('submit', handleReviewSubmit);
    }

    // 2. Gắn Listener cho Xem trước ảnh (nếu tồn tại)
    const imageUpload = document.getElementById('review-image-upload');
    const previewContainer = document.getElementById('image-preview-container');

    if (imageUpload && previewContainer) {
        // ... (Giữ nguyên code)
        imageUpload.addEventListener('change', function () {
            previewContainer.innerHTML = ''; // Xóa preview cũ
            if (this.files) {
                Array.from(this.files).forEach(file => {
                    if (file.type.startsWith('image/')) {
                        const reader = new FileReader();
                        reader.onload = function (e) {
                            const img = document.createElement('img');
                            img.src = e.target.result;
                            img.classList.add('image-preview-item');
                            previewContainer.appendChild(img);
                        };
                        reader.readAsDataURL(file);
                    }
                });
            }
        });
    }

    // 3. Gắn Listener "cha" cho toàn bộ trang (Event Delegation)
    document.addEventListener('click', function (e) {

        // 3a. BẬT/TẮT KHU VỤC COMMENT
        const toggleButton = e.target.closest('.btn-toggle-comment');
        if (toggleButton) {
            e.preventDefault();
            const reviewId = toggleButton.dataset.reviewId;
            const commentSection = document.getElementById(`comment-section-${reviewId}`);
            if (commentSection) {
                commentSection.classList.toggle('hidden');
            }
            return;
        }

        // 3b. GỬI COMMENT
        const submitCommentButton = e.target.closest('.btn-submit-comment');
        if (submitCommentButton) {
            e.preventDefault();
            const commentForm = submitCommentButton.closest('.comment-form');
            if (commentForm) {
                handleCommentSubmit(commentForm);
            }
            return;
        }

        // 3c. REACTION CHO REVIEW (HỮU ÍCH / BÁO CÁO)
        const reviewReactionButton = e.target.closest('.btn-toggle-reaction');
        if (reviewReactionButton) {
            e.preventDefault();
            handleReviewReactionToggle(reviewReactionButton);
            return;
        }

        // 3d. SỬA REVIEW (Bấm nút "Sửa")
        const editReviewButton = e.target.closest('.btn-edit-review');
        if (editReviewButton) {
            e.preventDefault();
            handleEditReviewClick(editReviewButton);
            return;
        }

        // 3e. HỦY SỬA REVIEW (Bấm nút "Hủy")
        const cancelReviewButton = e.target.closest('.btn-cancel-inline');
        if (cancelReviewButton) {
            e.preventDefault();
            handleCancelReviewClick(cancelReviewButton);
            return;
        }

        // 3f. SỬA COMMENT (Bấm nút "Sửa")
        const editCommentButton = e.target.closest('.btn-edit-comment');
        if (editCommentButton) {
            e.preventDefault();
            handleEditCommentClick(editCommentButton);
            return;
        }

        // 3g. REACTION CHO COMMENT (THÍCH / BÁO CÁO)
        const commentReactionButton = e.target.closest('.btn-react-comment');
        if (commentReactionButton) {
            e.preventDefault();
            handleCommentReactionClick(commentReactionButton);
            return;
        }

        // === THÊM KHỐI 3h MỚI ===
        // 3h. XÓA COMMENT (Bấm nút "Xóa")
        const deleteCommentButton = e.target.closest('.btn-delete-comment');
        if (deleteCommentButton) {
            e.preventDefault();
            handleDeleteCommentClick(deleteCommentButton);
            return;
        }
    });
}

// --- ĐIỂM BẮT ĐẦU: Chạy hàm cài đặt khi trang tải xong ---
document.addEventListener("DOMContentLoaded", setupEventListeners);