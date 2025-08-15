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

    document.querySelectorAll('.stat-number[data-count]').forEach(el => {
        const end = parseInt(el.getAttribute('data-count'));
        if (!isNaN(end)) {
            animateValue(el, 0, end, 2000);
        }
    });
});
