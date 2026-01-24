window.clickRadzenDropdownClear = (id) => {
    const clearButton = document.querySelector(`#${id} .rz-dropdown-clear-icon`);
    if (clearButton) clearButton.click();
};
