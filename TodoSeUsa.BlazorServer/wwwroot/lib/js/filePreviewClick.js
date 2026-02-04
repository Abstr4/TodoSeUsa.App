window.filePreviewClick = {
    attach: (containerId, dotnetRef) => {
        const container = document.getElementById(containerId);
        if (!container) return;

        container.querySelectorAll('.rz-fileupload-row img').forEach(img => {
            if (!img.dataset.clickAttached) {
                img.style.cursor = 'pointer';
                img.onclick = () => dotnetRef.invokeMethodAsync('ShowLargePreview', img.src);
                img.dataset.clickAttached = "true";
            }
        });
    }
};