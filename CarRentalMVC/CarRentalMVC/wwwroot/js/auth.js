// Password Toggle Functionality
function togglePassword(button) {
    const input = button.parentElement.querySelector(".password-input")
    const icon = button.querySelector("i")

    if (input.type === "password") {
        input.type = "text"
        icon.classList.remove("bi-eye")
        icon.classList.add("bi-eye-slash")
    } else {
        input.type = "password"
        icon.classList.remove("bi-eye-slash")
        icon.classList.add("bi-eye")
    }
}

// Form Validation Enhancement
document.addEventListener("DOMContentLoaded", () => {
    const forms = document.querySelectorAll(".auth-form")

    forms.forEach((form) => {
        const inputs = form.querySelectorAll(".form-control-custom")

        // Add focus/blur effects
        inputs.forEach((input) => {
            input.addEventListener("focus", function () {
                this.parentElement.classList.add("focused")
            })

            input.addEventListener("blur", function () {
                this.parentElement.classList.remove("focused")
                validateInput(this)
            })

            // Real-time validation
            input.addEventListener("input", function () {
                if (this.value.length > 0) {
                    validateInput(this)
                }
            })
        })

        // Form submission
        form.addEventListener("submit", (e) => {
            let isValid = true

            inputs.forEach((input) => {
                if (!validateInput(input)) {
                    isValid = false
                }
            })

            if (isValid) {
                const submitBtn = form.querySelector(".btn-auth-primary")
                submitBtn.classList.add("loading")
            }
        })
    })
})

// Input Validation Function
function validateInput(input) {
    const value = input.value.trim()
    const type = input.type
    const name = input.name || input.getAttribute("asp-for")
    let isValid = true
    let errorMessage = ""

    // Remove previous error
    const existingError = input.parentElement.parentElement.querySelector(".validation-message")
    if (existingError && !existingError.hasAttribute("data-valmsg-for")) {
        existingError.remove()
    }

    // Required field check
    if (input.hasAttribute("required") && value === "") {
        isValid = false
        errorMessage = "Trường này là bắt buộc"
    }

    // Email validation
    if (type === "email" && value !== "") {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        if (!emailRegex.test(value)) {
            isValid = false
            errorMessage = "Email không hợp lệ"
        }
    }

    // Phone validation
    if (type === "tel" && value !== "") {
        const phoneRegex = /^[0-9]{10,11}$/
        if (!phoneRegex.test(value.replace(/\s/g, ""))) {
            isValid = false
            errorMessage = "Số điện thoại không hợp lệ"
        }
    }

    // Password validation
    if (
        type === "password" &&
        value !== "" &&
        name &&
        name.toLowerCase().includes("password") &&
        !name.toLowerCase().includes("confirm")
    ) {
        if (value.length < 6) {
            isValid = false
            errorMessage = "Mật khẩu phải có ít nhất 6 ký tự"
        }
    }

    // Confirm password validation
    if (name && name.toLowerCase().includes("confirmpassword")) {
        const passwordInput = document.querySelector('input[name*="Password"]:not([name*="Confirm"])')
        if (passwordInput && value !== passwordInput.value) {
            isValid = false
            errorMessage = "Mật khẩu không khớp"
        }
    }

    // Update UI
    if (!isValid && errorMessage) {
        input.classList.add("is-invalid")
        input.classList.remove("is-valid")

        // Add error message if not exists
        if (!input.parentElement.parentElement.querySelector(".validation-message[data-valmsg-for]")) {
            const errorSpan = document.createElement("span")
            errorSpan.className = "text-danger validation-message"
            errorSpan.textContent = errorMessage
            input.parentElement.parentElement.appendChild(errorSpan)
        }
    } else if (value !== "") {
        input.classList.remove("is-invalid")
        input.classList.add("is-valid")
    } else {
        input.classList.remove("is-invalid", "is-valid")
    }

    return isValid
}

// Ripple Effect for Buttons
document.addEventListener("DOMContentLoaded", () => {
    const buttons = document.querySelectorAll(".btn-auth, .btn-social")

    buttons.forEach((button) => {
        button.addEventListener("click", function (e) {
            const ripple = document.createElement("span")
            const rect = this.getBoundingClientRect()
            const size = Math.max(rect.width, rect.height)
            const x = e.clientX - rect.left - size / 2
            const y = e.clientY - rect.top - size / 2

            ripple.style.width = ripple.style.height = size + "px"
            ripple.style.left = x + "px"
            ripple.style.top = y + "px"
            ripple.classList.add("ripple-effect")

            this.appendChild(ripple)

            setTimeout(() => {
                ripple.remove()
            }, 600)
        })
    })
})

// Add ripple effect styles dynamically
const style = document.createElement("style")
style.textContent = `
    .ripple-effect {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(2);
            opacity: 0;
        }
    }
    
    .is-invalid {
        border-color: #f56565 !important;
    }
    
    .is-valid {
        border-color: #48bb78 !important;
    }
`
document.head.appendChild(style)

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
    anchor.addEventListener("click", function (e) {
        e.preventDefault()
        const target = document.querySelector(this.getAttribute("href"))
        if (target) {
            target.scrollIntoView({
                behavior: "smooth",
                block: "start",
            })
        }
    })
})

// Auto-hide alerts after 5 seconds
document.addEventListener("DOMContentLoaded", () => {
    const alerts = document.querySelectorAll(".auth-alert")
    alerts.forEach((alert) => {
        setTimeout(() => {
            alert.style.animation = "slideUp 0.5s ease-out forwards"
            setTimeout(() => {
                alert.remove()
            }, 500)
        }, 5000)
    })
})
