document.addEventListener('DOMContentLoaded', function () {
    // Kiểm tra sự tồn tại của các phần tử
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');

    if (!searchInput || !searchResults) {
        console.error('Không tìm thấy #searchInput hoặc #searchResults trong DOM');
        return;
    }

    let selectedIndex = -1; // Chỉ số của gợi ý đang được chọn

    // Xử lý sự kiện input
    searchInput.addEventListener('input', async function () {
        const query = searchInput.value.trim();
        if (query.length < 1) {
            searchResults.style.display = 'none';
            searchResults.innerHTML = '';
            selectedIndex = -1;
            return;
        }

        try {
            console.log(`Gửi yêu cầu tìm kiếm với query: ${query}`); // Debug
            const response = await fetch(`/Home/Search?query=${encodeURIComponent(query)}`);
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            const products = await response.json();
            console.log('Kết quả tìm kiếm:', products); // Debug

            searchResults.innerHTML = '';
            selectedIndex = -1;

            if (!products || products.length === 0) {
                const item = document.createElement('li');
                item.classList.add('list-group-item');
                item.textContent = 'Không tìm thấy kết quả';
                searchResults.appendChild(item);
            } else {
                products.forEach((product, index) => {
                    const item = document.createElement('li');
                    item.classList.add('list-group-item', 'd-flex', 'align-items-center');

                    const img = document.createElement('img');
                    img.src = product.imageUrl || '/images/placeholder.jpg'; // Fallback nếu không có hình
                    img.alt = product.name || 'Product';
                    img.style.width = '40px';
                    img.style.height = '40px';
                    img.style.objectFit = 'cover';
                    img.style.borderRadius = '4px';
                    img.classList.add('me-3');

                    const text = document.createElement('span');
                    text.textContent = product.name || 'Unknown Product';

                    item.appendChild(img);
                    item.appendChild(text);

                    // Gắn data-index để theo dõi vị trí
                    item.dataset.index = index;

                    // Gắn sự kiện click cho từng mục
                    item.addEventListener('click', function () {
                        searchInput.value = product.name;
                        searchResults.style.display = 'none';
                        window.location.href = `/Product/Display/${product.id}`;
                    });

                    searchResults.appendChild(item);
                });
            }
            searchResults.style.display = 'block';
        } catch (error) {
            console.error('Lỗi khi fetch dữ liệu:', error);
            searchResults.innerHTML = '<li class="list-group-item">Đã có lỗi xảy ra</li>';
            searchResults.style.display = 'block';
        }
    });

    // Xử lý phím (mũi tên, Enter, Tab, Escape)
    searchInput.addEventListener('keydown', function (event) {
        const items = searchResults.querySelectorAll('.list-group-item');
        if (items.length === 0) return;

        if (event.key === 'ArrowDown') {
            event.preventDefault();
            if (selectedIndex < items.length - 1) {
                selectedIndex++;
                updateSelection(items);
            }
        } else if (event.key === 'ArrowUp') {
            event.preventDefault();
            if (selectedIndex > 0) {
                selectedIndex--;
                updateSelection(items);
            }
        } else if (event.key === 'Enter') {
            event.preventDefault();
            if (selectedIndex >= 0 && selectedIndex < items.length) {
                items[selectedIndex].click();
            } else {
                // Nếu không có gợi ý được chọn, có thể submit form tìm kiếm
                console.log('Submit form tìm kiếm với query:', searchInput.value);
                // Ví dụ: window.location.href = `/Product/Search?query=${encodeURIComponent(searchInput.value)}`;
            }
        } else if (event.key === 'Tab') {
            event.preventDefault();
            if (selectedIndex >= 0 && selectedIndex < items.length) {
                items[selectedIndex].click();
            }
        } else if (event.key === 'Escape') {
            searchResults.style.display = 'none';
            selectedIndex = -1;
        }
    });

    // Hàm cập nhật trạng thái gợi ý được chọn
    function updateSelection(items) {
        items.forEach((item, index) => {
            if (index === selectedIndex) {
                item.classList.add('active');
                item.scrollIntoView({ block: 'nearest' });
            } else {
                item.classList.remove('active');
            }
        });
    }

    // Ẩn danh sách gợi ý khi click ra ngoài
    document.addEventListener('click', function (event) {
        if (!searchInput.contains(event.target) && !searchResults.contains(event.target)) {
            searchResults.style.display = 'none';
            selectedIndex = -1;
        }
    });
});