const searchForm = document.querySelector(".search-form");
const cartItem = document.querySelector(".cart-items-container");
const navbar = document.querySelector(".navbar");

//! buttons
const searchBtn = document.querySelector("#search-btn");
const cartBtn = document.querySelector("#cart-btn");
const menuBtn = document.querySelector("#menu-btn");

searchBtn.addEventListener("click", function () {
    searchForm.classList.toggle("active");
    document.addEventListener("click", function (e) {
        if (
            !e.composedPath().includes(searchBtn) &&
            !e.composedPath().includes(searchForm)
        ) {
            searchForm.classList.remove("active");
        }
    });
});

cartBtn.addEventListener("click", function () {
    cartItem.classList.toggle("active");
    document.addEventListener("click", function (e) {
        if (
            !e.composedPath().includes(cartBtn) &&
            !e.composedPath().includes(cartItem)
        ) {
            cartItem.classList.remove("active");
        }
    });
});

menuBtn.addEventListener("click", function () {
    navbar.classList.toggle("active");
    document.addEventListener("click", function (e) {
        if (
            !e.composedPath().includes(menuBtn) &&
            !e.composedPath().includes(navbar)
        ) {
            navbar.classList.remove("active");
        }
    });
});
// Sepete ekleme fonksiyonu - GİRİŞSİZ VERSİYON
function addToCart(event, productId, productName, productPrice, productImage) {
    event.preventDefault();

    // Giriş kontrolünü KALDIRIYORUZ - direkt sepete ekliyoruz
    let cart = JSON.parse(localStorage.getItem('cart')) || [];

    // Ürün sepette var mı kontrol et
    const existingProduct = cart.find(item => item.id === productId);

    if (existingProduct) {
        existingProduct.quantity += 1;
    } else {
        cart.push({
            id: productId,
            name: productName,
            price: productPrice,
            image: productImage,
            quantity: 1
        });
    }

    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartCount();
    showSuccessMessage('Ürün sepete eklendi!');

    return false;
}

// Başarı mesajı (SweetAlert ile)
function showSuccessMessage(message) {
    // SweetAlert varsa
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            title: 'Başarılı!',
            text: message,
            icon: 'success',
            timer: 1500,
            showConfirmButton: false
        });
    } else {
        // SweetAlert yoksa normal alert
        alert(message);
    }
}
