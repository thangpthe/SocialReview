//// Chạy khi tài liệu (HTML) đã tải xong
//document.addEventListener("DOMContentLoaded", function () {

//    // Lấy token Chống giả mạo (bắt buộc)
//    // Token này được render bởi @Html.AntiForgeryToken() trong form
//    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

//    // Nếu không tìm thấy token, báo lỗi và dừng lại
//    if (!tokenInput) {
//        console.error("LỖI BẢO MẬT: Không tìm thấy __RequestVerificationToken. Hãy thêm @Html.AntiForgeryToken() vào form 'form-create-review'.");
//        return;
//    }
//    const token = tokenInput.value;


//    // --- XỬ LÝ FORM VIẾT REVIEW (AJAX) ---
//    const reviewForm = document.getElementById('form-create-review');
//    const reviewFormError = document.getElementById('review-form-error');
//    const submitReviewButton = document.getElementById('btn-submit-review');

//    if (reviewForm) {
//        reviewForm.addEventListener('submit', function (e) {
//            // 1. Ngăn form gửi đi theo cách truyền thống (tải lại trang)
//            e.preventDefault();

//            const originalButtonText = submitReviewButton.innerHTML;
//            submitReviewButton.innerHTML = "Đang gửi...";
//            submitReviewButton.disabled = true;
//            reviewFormError.classList.add('hidden'); // Ẩn lỗi cũ

//            // 3. Gửi dữ liệu bằng fetch (AJAX)
//            fetch(reviewForm.action, {
//                method: 'POST',
//                body: new FormData(reviewForm), // Gửi dữ liệu form
//                headers: {
//                    // 4. Gửi token trong header
//                    'RequestVerificationToken': token
//                }
//            })
//                .then(response => {
//                    // 5a. Nếu thành công (Server trả về 200)
//                    if (response.ok) {
//                        return response.text(); // Lấy "mẩu HTML" (PartialView)
//                    }

//                    // 5b. Nếu thất bại (Server trả về 400 - BadRequest)
//                    // Cố gắng đọc JSON, nhưng chuẩn bị cho trường hợp thất bại
//                    return response.json().catch(() => {
//                        // Nếu server trả về lỗi 500 (HTML) hoặc lỗi không phải JSON
//                        throw new Error("Lỗi máy chủ, không thể gửi đánh giá.");
//                    }).then(errorData => {
//                        // Ném lỗi (catch bên dưới sẽ bắt)
//                        throw new Error(errorData.errors.join(', '));
//                    });
//                })
//                .then(html => {
//                    // 6. Thành công! Chèn "mẩu HTML" (review mới) vào danh sách
//                    const reviewsList = document.querySelector('.reviews-list');

//                    // Xóa "Chưa có đánh giá nào" (nếu có)
//                    const noReviewsMsg = reviewsList.querySelector('.card.p-5.text-center');
//                    if (noReviewsMsg) {
//                        noReviewsMsg.remove();
//                    }

//                    // Thêm review mới vào đầu danh sách
//                    reviewsList.insertAdjacentHTML('afterbegin', html);

//                    // Xóa nội dung form
//                    reviewForm.reset();
//                })
//                .catch(error => {
//                    // 7. Thất bại! Hiển thị lỗi
//                    // --- SỬA LỖI ---
//                    console.error("Lỗi AJAX (Viết Review):", error.message); // 1. Log lỗi kỹ thuật cho lập trình viên
//                    reviewFormError.textContent = "Đã xảy ra lỗi. Vui lòng thử lại sau."; // 2. Hiển thị lỗi thân thiện
//                    reviewFormError.classList.remove('hidden');
//                })
//                .finally(() => {
//                    // 8. Luôn luôn: Khôi phục nút bấm
//                    submitReviewButton.innerHTML = originalButtonText;
//                    submitReviewButton.disabled = false;
//                });
//        });
//    }


//    // --- XỬ LÝ TƯƠNG TÁC (DELEGATION) CHO REVIEW VÀ COMMENT ---
//    const reviewsSection = document.getElementById('reviews');
//    if (reviewsSection) {
//        reviewsSection.addEventListener('click', function (e) {

//            // 1. XỬ LÝ BẬT/TẮT KHU VỤC COMMENT
//            const toggleButton = e.target.closest('.btn-toggle-comment');
//            if (toggleButton) {
//                e.preventDefault();
//                const reviewId = toggleButton.dataset.reviewId;
//                const commentSection = document.getElementById(`comment-section-${reviewId}`);
//                if (commentSection) {
//                    commentSection.classList.toggle('hidden');
//                }
//            }

//            // 2. XỬ LÝ GỬI COMMENT
//            const submitCommentButton = e.target.closest('.btn-submit-comment');
//            if (submitCommentButton) {
//                e.preventDefault();
//                const commentForm = submitCommentButton.closest('.comment-form');
//                handleCommentSubmit(commentForm, token); // Truyền token
//            }

//            // 3. XỬ LÝ GỬI REACTION (HỮU ÍCH)
//            const reactionButton = e.target.closest('.btn-toggle-reaction');
//            if (reactionButton) {
//                e.preventDefault();
//                handleReactionToggle(reactionButton, token); // Truyền token
//            }
//        });
//    }

//    // --- HÀM XỬ LÝ FORM VIẾT COMMENT (AJAX) ---
//    function handleCommentSubmit(commentForm, antiForgeryToken) {

//        const reviewId = commentForm.dataset.reviewId;
//        const errorDiv = document.getElementById(`comment-error-${reviewId}`);
//        const commentList = document.getElementById(`comment-list-${reviewId}`);
//        const commentInput = commentForm.querySelector('input[name="Content"]');

//        // (Kiểm tra xem input có rỗng không)
//        if (!commentInput.value) {
//            errorDiv.textContent = "Vui lòng nhập nội dung bình luận.";
//            errorDiv.classList.remove('hidden');
//            return;
//        }

//        errorDiv.classList.add('hidden'); // Ẩn lỗi cũ

//        fetch(commentForm.action, {
//            method: 'POST',
//            body: new FormData(commentForm),
//            headers: {
//                'RequestVerificationToken': antiForgeryToken
//            }
//        })
//            .then(response => {
//                if (response.ok) {
//                    return response.text(); // Lấy "mẩu HTML"
//                }
//                // Cố gắng đọc JSON, nhưng chuẩn bị cho trường hợp thất bại
//                return response.json().catch(() => {
//                    throw new Error("Lỗi máy chủ, không thể gửi bình luận.");
//                }).then(errorData => {
//                    throw new Error(errorData.errors.join(', '));
//                });
//            })
//            .then(html => {
//                // Thành công!
//                // Xóa "Chưa có bình luận" (nếu có)
//                const noCommentsMsg = commentList.querySelector('.no-comments');
//                if (noCommentsMsg) {
//                    noCommentsMsg.remove();
//                }

//                // Thêm bình luận mới vào danh sách
//                commentList.insertAdjacentHTML('beforeend', html);

//                // Xóa nội dung input
//                commentInput.value = '';
//            })
//            .catch(error => {
//                // Thất bại!
//                // --- SỬA LỖI (Dòng bạn đã chỉ ra) ---
//                console.error("Lỗi AJAX (Gửi Comment):", error.message); // 1. Log lỗi kỹ thuật
//                errorDiv.textContent = "Đã xảy ra lỗi khi gửi bình luận."; // 2. Hiển thị lỗi thân thiện
//                errorDiv.classList.remove('hidden');
//            });
//    }


//    // --- HÀM MỚI: XỬ LÝ REACTION (HỮU ÍCH) ---
//    function handleReactionToggle(reactionButton, antiForgeryToken) {
//        const reviewId = reactionButton.dataset.reviewId;
//        const reactionType = reactionButton.dataset.type || "Helpful";

//        // Tắt nút bấm
//        reactionButton.disabled = true;

//        // Chuẩn bị dữ liệu form
//        const formData = new FormData();
//        formData.append('reviewId', reviewId);
//        formData.append('reactionType', reactionType);

//        // --- SỬA LỖI: Xóa dòng 'formData.append('__RequestVerificationToken'...)' ---


//        fetch('/api/reaction/toggle', { // Gọi API mới
//            method: 'POST',
//            body: formData,

//            // --- SỬA LỖI: Thêm khối 'headers' (giống hệt hàm Comment) ---
//            headers: {
//                'RequestVerificationToken': antiForgeryToken
//            }
//        })
//            .then(response => {
//                if (response.ok) {
//                    return response.json(); // Lấy dữ liệu JSON (newCount)
//                }
//                if (response.status === 401) { // Lỗi chưa đăng nhập
//                    throw new Error('Bạn cần đăng nhập để thực hiện việc này.');
//                }
//                throw new Error('Lỗi máy chủ khi reaction.');
//            })
//            .then(data => {
//                // Thành công! Cập nhật số lượng
//                const countSpan = reactionButton.querySelector('span');
//                if (countSpan) {
//                    countSpan.textContent = `(${data.newCount})`;
//                }
//                // TODO: Đổi màu nút nếu người dùng hiện tại đã Like
//                reactionButton.classList.toggle('text-purple-600');
//            })
//            .catch(error => {
//                // --- SỬA LỖI ---
//                console.error('Lỗi AJAX Reaction:', error.message); // 1. Log lỗi
//                alert("Đã xảy ra lỗi: " + error.message); // 2. Hiển thị lỗi (thân thiện hơn một chút)
//                // (Chúng ta dùng alert ở đây vì không có div lỗi riêng cho nút reaction)
//            })
//            .finally(() => {
//                // Bật lại nút bấm
//                reactionButton.disabled = false;
//            });
//    }

//});

/**
 * product_detail.js
 * * Xử lý toàn bộ logic AJAX (JavaScript thuần) cho:
 * 1. Gửi Đánh giá (Review) mới.
 * 2. Bật/Tắt và Gửi Bình luận (Comment) mới.
 * 3. Gửi Phản ứng (Reaction - Hữu ích).
 *
 * Đã được tái cấu trúc (refactored) để tách riêng các hàm.
 */

// --- BIẾN TOÀN CỤC (GLOBAL HELPERS) ---

// Lấy token bảo mật từ thẻ meta (được render bởi _Layout.cshtml)
// Cập nhật: Lấy từ form logout, giả sử nó luôn có
const antiForgeryTokenEl = document.querySelector('form[action="/Auth/Logout"] input[name="__RequestVerificationToken"]');
const antiForgeryToken = antiForgeryTokenEl ? antiForgeryTokenEl.value : null;

// Kiểm tra xem người dùng đã đăng nhập hay chưa (đọc từ thẻ body)
const isAuthenticated = document.body.dataset.isAuthenticated === 'true';

/**
 * Hàm kiểm tra bảo vệ:
 * Nếu chưa đăng nhập, chuyển hướng đến trang Login.
 * @returns {boolean} - Trả về 'true' nếu đã đăng nhập (an toàn), 'false' nếu đã chuyển hướng (không an toàn).
 */
function checkAuthentication() {
    if (!isAuthenticated) {
        // Nếu chưa đăng nhập, chuyển hướng đến trang Login
        window.location.href = '/Auth/Login?ReturnUrl=' + encodeURIComponent(window.location.pathname);
        return false;
    }
    if (!antiForgeryToken) {
        // Lỗi nghiêm trọng: Trang (Layout) không render token
        console.error("LỖI BẢO MẬT: Không tìm thấy __RequestVerificationToken.");
        alert("Lỗi bảo mật, không thể thực hiện. Vui lòng tải lại trang.");
        return false;
    }
    return true;
}

/**
 * Hiển thị lỗi chung (generic) trên một ô lỗi
 * @param {HTMLElement} errorElement - Thẻ div để hiển thị lỗi.
 * @param {string} technicalError - Lỗi kỹ thuật (để log).
 * @param {string} userMessage - Lỗi thân thiện (để hiển thị).
 */
function displayError(errorElement, technicalError, userMessage) {
    if (errorElement) {
        console.error(technicalError); // 1. Log lỗi kỹ thuật cho lập trình viên
        errorElement.textContent = userMessage; // 2. Hiển thị lỗi thân thiện
        errorElement.classList.remove('hidden');
    } else {
        // Fallback nếu không tìm thấy ô lỗi
        console.error(technicalError);
        alert(userMessage);
    }
}

// --- HÀM 1: XỬ LÝ GỬI REVIEW MỚI ---
async function handleReviewSubmit(e) {
    // 1. Ngăn form gửi đi theo cách truyền thống
    e.preventDefault();
    // 2. Kiểm tra bảo vệ
    if (!checkAuthentication()) return;

    const reviewForm = e.target;
    const reviewFormError = document.getElementById('review-form-error');
    const submitReviewButton = document.getElementById('btn-submit-review');

    if (!submitReviewButton || !reviewFormError) return; // Thoát nếu thiếu phần tử

    const originalButtonText = submitReviewButton.innerHTML;
    submitReviewButton.innerHTML = "Đang gửi...";
    submitReviewButton.disabled = true;
    reviewFormError.classList.add('hidden'); // Ẩn lỗi cũ

    try {
        // 3. Gửi dữ liệu bằng fetch (AJAX)
        const response = await fetch(reviewForm.action, {
            method: 'POST',
            body: new FormData(reviewForm), // Gửi dữ liệu form
            headers: {
                'RequestVerificationToken': antiForgeryToken
            }
        });

        // 4a. Nếu thất bại (lỗi 400 - validation, 500 - server)
        if (!response.ok) {
            const errorData = await response.json().catch(() => {
                // Nếu server trả về lỗi 500 (HTML) hoặc lỗi không phải JSON
                throw new Error("Lỗi máy chủ, không thể gửi đánh giá.");
            });
            throw new Error(errorData.errors.join(', '));
        }

        // 4b. Nếu thành công (lỗi 200)
        const html = await response.text(); // Lấy "mẩu HTML" (PartialView)

        // 5. Chèn "mẩu HTML" (review mới) vào danh sách
        const reviewsList = document.querySelector('.reviews-list');

        if (reviewsList) {
            // Xóa "Chưa có đánh giá nào" (nếu có)
            const noReviewsMsg = reviewsList.querySelector('.card.p-5.text-center');
            if (noReviewsMsg) {
                noReviewsMsg.remove();
            }

            // Thêm review mới vào đầu danh sách
            reviewsList.insertAdjacentHTML('afterbegin', html);

            // Xóa nội dung form
            reviewForm.reset();
        }

    } catch (error) {
        // 6. Thất bại! Hiển thị lỗi
        displayError(reviewFormError, `Lỗi AJAX (Viết Review): ${error.message}`, "Đã xảy ra lỗi. Vui lòng thử lại sau.");
    } finally {
        // 7. Luôn luôn: Khôi phục nút bấm
        submitReviewButton.innerHTML = originalButtonText;
        submitReviewButton.disabled = false;
    }
}

// --- HÀM 2: XỬ LÝ GỬI COMMENT MỚI ---
async function handleCommentSubmit(commentForm) {
    // 1. Kiểm tra bảo vệ
    if (!checkAuthentication()) return;

    const reviewId = commentForm.dataset.reviewId;
    const errorDiv = document.getElementById(`comment-error-${reviewId}`);
    const commentList = document.getElementById(`comment-list-${reviewId}`);
    const commentInput = commentForm.querySelector('input[name="Content"]');

    if (!errorDiv || !commentList || !commentInput) return; // Thoát nếu thiếu

    // (Kiểm tra xem input có rỗng không)
    if (!commentInput.value) {
        displayError(errorDiv, "Comment input is empty.", "Vui lòng nhập nội dung bình luận.");
        return;
    }

    errorDiv.classList.add('hidden'); // Ẩn lỗi cũ
    const submitButton = commentForm.querySelector('.btn-submit-comment');
    if (submitButton) submitButton.disabled = true;

    try {
        // 2. Gửi AJAX
        const response = await fetch(commentForm.action, {
            method: 'POST',
            body: new FormData(commentForm),
            headers: {
                'RequestVerificationToken': antiForgeryToken
            }
        });

        // 3a. Thất bại
        if (!response.ok) {
            const errorData = await response.json().catch(() => {
                throw new Error("Lỗi máy chủ, không thể gửi bình luận.");
            });
            throw new Error(errorData.errors.join(', '));
        }

        // 3b. Thành công
        const html = await response.text(); // Lấy "mẩu HTML"

        // Xóa "Chưa có bình luận" (nếu có)
        const noCommentsMsg = commentList.querySelector('.no-comments');
        if (noCommentsMsg) {
            noCommentsMsg.remove();
        }

        // Thêm bình luận mới vào danh sách
        commentList.insertAdjacentHTML('beforeend', html);

        // Xóa nội dung input
        commentInput.value = '';

    } catch (error) {
        // 4. Hiển thị lỗi
        displayError(errorDiv, `Lỗi AJAX (Gửi Comment): ${error.message}`, "Đã xảy ra lỗi khi gửi bình luận.");
    } finally {
        if (submitButton) submitButton.disabled = false;
    }
}

// --- HÀM 3: XỬ LÝ REACTION (HỮU ÍCH) ---
async function handleReactionToggle(reactionButton) {
    // 1. Kiểm tra bảo vệ
    if (!checkAuthentication()) return;

    const reviewId = reactionButton.dataset.reviewId;
    const reactionType = reactionButton.dataset.type || "Helpful";

    reactionButton.disabled = true; // Tắt nút bấm

    // 2. Chuẩn bị dữ liệu
    const formData = new FormData();
    formData.append('reviewId', reviewId);
    formData.append('reactionType', reactionType);

    try {
        // 3. Gửi AJAX
        const response = await fetch('/api/reaction/toggle', { // Gọi API
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': antiForgeryToken
            }
        });

        if (!response.ok) {
            throw new Error('Lỗi máy chủ khi reaction.');
        }

        const data = await response.json(); // Lấy dữ liệu JSON (newCount, userHasReacted)

        // 4. Thành công! Cập nhật số lượng
        const countSpan = reactionButton.querySelector('span');
        if (countSpan) {
            countSpan.textContent = `(${data.newCount})`;
        }

        // Đổi màu nút nếu người dùng hiện tại đã Like
        if (data.userHasReacted) {
            reactionButton.classList.add('active'); // Thêm class 'active' (cần CSS)
        } else {
            reactionButton.classList.remove('active'); // Bỏ class 'active'
        }

    } catch (error) {
        // 5. Thất bại
        console.error('Lỗi AJAX Reaction:', error.message); // 1. Log lỗi
        alert("Đã xảy ra lỗi: " + error.message); // 2. Hiển thị lỗi
    } finally {
        reactionButton.disabled = false; // Bật lại nút bấm
    }
}


// --- HÀM 4: CÀI ĐẶT CÁC EVENT LISTENER ---
function setupEventListeners() {

    // 1. Gắn Listener cho Form Viết Review (nếu nó tồn tại)
    const reviewForm = document.getElementById('form-create-review');
    if (reviewForm) {
        reviewForm.addEventListener('submit', handleReviewSubmit);
    }

    // 2. Gắn Listener "cha" cho toàn bộ trang (Event Delegation)
    // (Dùng 'document' để bắt sự kiện ở cả Trang chủ và Trang chi tiết)
    document.addEventListener('click', function (e) {

        // 2a. XỬ LÝ BẬT/TẮT KHU VỤC COMMENT
        const toggleButton = e.target.closest('.btn-toggle-comment');
        if (toggleButton) {
            e.preventDefault();
            const reviewId = toggleButton.dataset.reviewId;
            const commentSection = document.getElementById(`comment-section-${reviewId}`);
            if (commentSection) {
                commentSection.classList.toggle('hidden');
            }
            return; // Dừng lại
        }

        // 2b. XỬ LÝ GỬI COMMENT
        const submitCommentButton = e.target.closest('.btn-submit-comment');
        if (submitCommentButton) {
            e.preventDefault();
            const commentForm = submitCommentButton.closest('.comment-form');

            // --- SỬA LỖI (THÊM CÂU LỆNH BẢO VỆ) ---
            // Kiểm tra xem commentForm có tồn tại không trước khi gọi
            if (commentForm) {
                handleCommentSubmit(commentForm); // Token đã ở global
            } else {
                console.error("Lỗi HTML: Nút .btn-submit-comment không nằm trong .comment-form");
            }
            return; // Dừng lại
        }

        // 2c. XỬ LÝ GỬI REACTION (HỮU ÍCH)
        const reactionButton = e.target.closest('.btn-toggle-reaction');
        if (reactionButton) {
            e.preventDefault();
            handleReactionToggle(reactionButton); // Token đã ở global
            return; // Dừng lại
        }
    });
}

// --- ĐIỂM BẮT ĐẦU: Chạy hàm cài đặt khi trang tải xong ---
document.addEventListener("DOMContentLoaded", setupEventListeners);

