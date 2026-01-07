function initScrollToTop() {
    let mybutton = document.getElementById("myBtn");

    window.addEventListener("scroll", scrollFunction);

    function scrollFunction() {
        if (window.scrollY > window.innerHeight) {
            mybutton.style.display = "block";
        } else {
            mybutton.style.display = "none";
        }
    }

    window.topFunction = function () {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
}