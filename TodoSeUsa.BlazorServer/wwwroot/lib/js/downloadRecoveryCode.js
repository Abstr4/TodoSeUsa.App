document.getElementById('downloadBtn')?.addEventListener('click', () => {
    const codeElement = document.getElementById('recoveryCode');
    const code = codeElement?.textContent;
    if (!code) return;

    const blob = new Blob([code], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = 'recovery-code.txt';
    a.click();

    URL.revokeObjectURL(url);
});
