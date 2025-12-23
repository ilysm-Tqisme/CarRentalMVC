// Admin Layout JavaScript

document.addEventListener("DOMContentLoaded", () => {
    // Sidebar toggle functionality
    const sidebarToggleBtn = document.getElementById("sidebarToggleBtn")
    const sidebarToggle = document.getElementById("sidebarToggle")
    const sidebar = document.getElementById("sidebar")

    // Create overlay element
    const overlay = document.createElement("div")
    overlay.className = "sidebar-overlay"
    document.body.appendChild(overlay)

    // Toggle sidebar on mobile
    if (sidebarToggleBtn) {
        sidebarToggleBtn.addEventListener("click", () => {
            sidebar.classList.toggle("show")
            overlay.classList.toggle("show")
        })
    }

    // Close sidebar when clicking close button
    if (sidebarToggle) {
        sidebarToggle.addEventListener("click", () => {
            sidebar.classList.remove("show")
            overlay.classList.remove("show")
        })
    }

    // Close sidebar when clicking overlay
    overlay.addEventListener("click", () => {
        sidebar.classList.remove("show")
        overlay.classList.remove("show")
    })

    // Active link highlighting
    const currentPath = window.location.pathname
    const navLinks = document.querySelectorAll(".sidebar-nav .nav-link")

    navLinks.forEach((link) => {
        if (link.getAttribute("href") === currentPath) {
            link.classList.add("active")
        }

        link.addEventListener("click", function () {
            navLinks.forEach((l) => l.classList.remove("active"))
            this.classList.add("active")
        })
    })

    // Auto-close sidebar on mobile when clicking a link
    navLinks.forEach((link) => {
        link.addEventListener("click", () => {
            if (window.innerWidth < 992) {
                sidebar.classList.remove("show")
                overlay.classList.remove("show")
            }
        })
    })

    // Handle window resize
    window.addEventListener("resize", () => {
        if (window.innerWidth >= 992) {
            sidebar.classList.remove("show")
            overlay.classList.remove("show")
        }
    })
})
