// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function showPopup(message, type = "success", duration = 3000) {
    const popup = document.getElementById("app-popup");
    const msg = document.getElementById("popup-message");

    if (!popup || !msg) {
        console.error("Popup HTML not found");
        return;
    }

    msg.textContent = message;

    popup.className = "popup " + type;
    popup.classList.remove("hidden");

    requestAnimationFrame(() => {
        popup.classList.add("show");
    });

    setTimeout(() => {
        popup.classList.remove("show");
        setTimeout(() => popup.classList.add("hidden"), 300);
    }, duration);
}