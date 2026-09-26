(() => {
    const root = document.documentElement;
    const savedTheme = localStorage.getItem('karim-doors-theme');
    if (savedTheme === 'dark' || savedTheme === 'light') {
        root.setAttribute('data-theme', savedTheme);
    }

    document.getElementById('themeToggle')?.addEventListener('click', () => {
        const next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-theme', next);
        localStorage.setItem('karim-doors-theme', next);
    });

    const sidebar = document.getElementById('sidebar');
    document.getElementById('menuToggle')?.addEventListener('click', () => sidebar?.classList.toggle('open'));

    document.addEventListener('click', event => {
        if (window.innerWidth > 760 || !sidebar?.classList.contains('open')) return;
        const target = event.target;
        if (target instanceof Node && !sidebar.contains(target) && target !== document.getElementById('menuToggle')) {
            sidebar.classList.remove('open');
        }
    });
})();
