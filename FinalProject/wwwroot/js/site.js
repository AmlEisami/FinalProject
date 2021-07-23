// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getCartQuantity() {
    console.log("naorr");
    let cart = JSON.parse(localStorage.getItem("cart"));
    if (cart == null) {
        $('.badge').text(0);
    } else {
        let count = 0;
        cart.map(item => count = count + Number(item.quan));
        $('.badge').text(count);
    }
}

$(function () {
    //$('#cart1').load(getCartQuantity);
    $('.add-to-cart').click(getCartQuantity);
});