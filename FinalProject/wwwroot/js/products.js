function AddToCart(itemId) {
    if (localStorage.getItem("cart") == null) {
        localStorage.setItem("cart", JSON.stringify([]));
    }
    let cart = JSON.parse(localStorage.getItem("cart"));
    let cartItemId = cart.findIndex(item => item.id === itemId);
    if (cartItemId === -1) {
        cart.push({ id: itemId, quan: Number($('#quantity').val()) })
    } else {
        cart[cartItemId] = { ...cart[cartItemId], quan: Number(cart[cartItemId].quan) + Number($('#quantity').val()) };
    }
    localStorage.setItem("cart", JSON.stringify(cart));
}

/*$(function () {
    $('.add-to-cart').click(function () {
        //localStorage.setItem("cart", )
        alert(Model);
    })
});*/