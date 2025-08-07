function checkAndRemoveEmptyHomeInfoContainer() {
    const container = document.getElementById('home-info-container');
    const containerHeader = document.getElementById('home-info-header');
    if (container && !container.querySelector('.home-info')) {
        container.parentElement.remove();
        containerHeader.remove();
    }
}

document.addEventListener('DOMContentLoaded', (event) => {
    document.querySelectorAll('.double-close-button').forEach(button => {
        button.addEventListener('click', () => {
            const closestHomeInfo = button.closest('.home-info');
            if (closestHomeInfo) {
                closestHomeInfo.remove();
                checkAndRemoveEmptyHomeInfoContainer();
            }
        });
    });
});