document.getElementById("registerForm").addEventListener("submit", function (event) {
    event.preventDefault();

    const formData = {
        fullName: document.getElementById("fullName").value,
        email: document.getElementById("email").value,
        userLogin: document.getElementById("login").value,
        password: document.getElementById("password").value,
        phone: document.getElementById("phone").value,
        address: document.getElementById("address").value,
        placeOfWork: document.getElementById("placeOfWork").value
    };

    const xmlData =
        `<?xml version="1.0" encoding="UTF-8"?>` +
        `<RegistrationRequest>` +
        `<fullName>${formData.fullName}</fullName>` +
        `<email>${formData.email}</email>` +
        `<phone>${formData.phone}</phone>` +
        `<address>${formData.address}</address>` +
        `<placeOfWork>${formData.placeOfWork}</placeOfWork>` +
        `<login>${formData.userLogin}</login>` +
        `<password>${formData.password}</password>` +
        `</RegistrationRequest>`;

    fetch("/api/Registration/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/xml"
        },
        body: xmlData
    })
        .then(response => {
        if (response.ok) {
            window.location.href = "login.html";
        } else {
            return response.text().then(text => {
                console.error("Server response:", text);
                alert("Ошибка регистрации");
            });
        }
    })
    .catch(error => {
        console.error("Error:", error);
        alert("Что-то пошло не так. Попробуйте позже");
    });
});