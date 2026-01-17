function copyRecoveryCode(code, button) {
    navigator.clipboard.writeText(code);

    button.classList.add("copied");

    setTimeout(() => {
        button.classList.remove("copied");
    }, 1400);
}
