document.addEventListener('DOMContentLoaded', () => {
    const locationInput = document.getElementById('location');
    const placeholders = ["Melbourne CBD", "Bondi Beach", "Brisbane North"];
    if (locationInput) {
        let index = 0;
        const cycle = () => {
            locationInput.setAttribute('placeholder', placeholders[index]);
            index = (index + 1) % placeholders.length;
        };
        cycle();
        setInterval(cycle, 3000);
    }

    const animateValue = (element, start, end, duration) => {
        const range = end - start;
        const increment = end > start ? 1 : -1;
        const stepTime = Math.abs(Math.floor(duration / range));
        let current = start;
        const timer = setInterval(() => {
            current += increment;
            element.textContent = current + "+";
            if (current === end) clearInterval(timer);
        }, stepTime);
    };

    const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const el = entry.target;
                const end = parseInt(el.getAttribute('data-count'));
                if (!isNaN(end)) {
                    animateValue(el, 0, end, 2000);
                }
                el.classList.add('visible');
                observer.unobserve(el);
            }
        });
    }, { threshold: 0.6 });

    document.querySelectorAll('.stat-number[data-count]').forEach(el => observer.observe(el));

    const backToTop = document.getElementById('backToTop');
    if (backToTop) {
        window.addEventListener('scroll', () => {
            backToTop.classList.toggle('show', window.scrollY > 300);
        });
        backToTop.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));
    }

    const mobileToggle = document.querySelector('.mobile-menu-toggle');
    const mobileMenu = document.querySelector('.mobile-nav');
    mobileToggle?.addEventListener('click', () => {
        mobileMenu?.classList.toggle('active');
        mobileToggle.classList.toggle('active');
    });
});

window.showToast = (message, type) => {
    const toastEl = document.getElementById('statusToast');
    if (toastEl) {
        toastEl.classList.remove('bg-success', 'bg-danger', 'text-white');
        toastEl.classList.add(type === 'success' ? 'bg-success' : 'bg-danger', 'text-white');
        toastEl.querySelector('.toast-body').textContent = message;
        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
};
