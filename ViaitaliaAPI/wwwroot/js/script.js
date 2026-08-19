// ================= Main Script =================
document.addEventListener('DOMContentLoaded', function () {
    // ================= Sticky Header & Scroll =================
    const stickyHeader = document.getElementById('stickyHeader');
    const heroSection = document.querySelector('.hero-section');
    const tripSection = document.querySelector('.trip-planning-section');
    const exploreSection = document.querySelector('.explore-section');
    const brandContainer = document.querySelector('.brand-container');
    const navButtons = document.querySelector('.nav-buttons'); // hero wale buttons

    function handleScroll() {
        if (window.scrollY > 200) {
            stickyHeader?.classList.add('active');
            heroSection?.classList.add('hero-hidden');
            tripSection?.classList.add('trip-hidden');
            exploreSection?.classList.add('explore-fixed');
            brandContainer?.classList.add('hidden');
            // ❌ isko hata do: navButtons?.classList.add('hidden');
        } else {
            stickyHeader?.classList.remove('active');
            heroSection?.classList.remove('hero-hidden');
            tripSection?.classList.remove('trip-hidden');
            exploreSection?.classList.remove('explore-fixed');
            brandContainer?.classList.remove('hidden');
            // ❌ isko hata do: navButtons?.classList.remove('hidden');
        }
    }


    let isThrottled = false;
    window.addEventListener('scroll', function () {
        if (!isThrottled) {
            handleScroll();
            isThrottled = true;
            setTimeout(() => (isThrottled = false), 100);
        }
    });
    handleScroll(); // initial check

    // ================= FAQ Dropdown =================
    const faqItems = document.querySelectorAll('.faq-item');
    faqItems.forEach(item => {
        const container = item.querySelector('.question-container');
        const arrowIcon = item.querySelector('.arrow-icon');
        const answer = item.querySelector('.faq-answer');
        if (!container || !arrowIcon || !answer) return;

        function toggleAnswer() {
            const isActive = answer.classList.contains('active');
            // Close all open answers first
            document.querySelectorAll('.faq-answer.active').forEach(openAns => {
                openAns.classList.remove('active');
                openAns.style.maxHeight = null;
                openAns.closest('.faq-item').querySelector('.arrow-icon').style.transform = 'rotate(0deg)';
            });
            if (!isActive) {
                answer.classList.add('active');
                answer.style.maxHeight = answer.scrollHeight + 'px'; // full height
                arrowIcon.style.transform = 'rotate(180deg)';
            }
        }

        container.addEventListener('click', toggleAnswer);
        arrowIcon.addEventListener('click', e => {
            e.stopPropagation();
            toggleAnswer();
        });
    });

    // ================= Header Buttons =================
    const loginBtns = document.querySelectorAll('.login-btn');
    const signinBtns = document.querySelectorAll('.signin-btn');
    const heroButton = document.querySelector('.hero-button');
    const ctaButton = document.querySelector('.cta-button');

    loginBtns.forEach(btn =>
        btn.addEventListener('click', () => (window.location.href = 'login.cshtml'))
    );
    signinBtns.forEach(btn =>
        btn.addEventListener('click', () => (window.location.href = 'register.cshtml'))
    );

    heroButton?.addEventListener('click', function () {
        this.style.transform = 'scale(0.95)';
        setTimeout(() => (this.style.transform = ''), 200);
        window.location.href = 'recommendation form.html';
    });
    ctaButton?.addEventListener('click', function () {
        this.style.transform = 'scale(0.95)';
        setTimeout(() => (this.style.transform = ''), 200);
        window.location.href = 'recommendation form.html';
    });

    // ================= Destination Cards =================
    document.querySelectorAll('.destination-card').forEach(card => {
        card.addEventListener('click', function () {
            const city = this.querySelector('.city-name').textContent;
            console.log(`Clicked: ${city}`);
        });
    });
    document.querySelectorAll('.card-icon').forEach(icon => {
        icon.addEventListener('click', function (e) {
            e.stopPropagation();
            this.classList.toggle('favorited');
            const city = this.closest('.destination-card').querySelector('.city-name').textContent;
            console.log(`${this.classList.contains('favorited') ? 'Added' : 'Removed'} ${city}`);
            this.style.filter = this.classList.contains('favorited')
                ? 'drop-shadow(0 0 5px rgba(255,0,0,0.7))'
                : '';
        });
    });

    // ================= Footer Links =================
    const footerLinks = document.querySelectorAll('.footer a');
    footerLinks.forEach(link =>
        link.addEventListener('click', e => {
            // ⚡ Prevent mat karo, direct open hone do
            window.location.href = link.getAttribute('href');
        })
    );
});
