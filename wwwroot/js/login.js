document.getElementById("loginForm").addEventListener("submit", function (event) {
    event.preventDefault();

    const formData = {
        login: document.getElementById("Login").value,
        password: document.getElementById("Password").value
    };

    fetch("/api/Authorization/authorize", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(formData)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    console.error("Server response:", text);
                    alert("Ошибка входа");
                });
            }
            return response.json();
        })
        .then(data => {
            if (data && data.status === "success") {
                localStorage.setItem("loggedInUser", JSON.stringify(data.user));
                window.location.href = data.redirectUrl;
            }
        })
        .catch(error => {
            console.error("Error:", error);
            alert("Что-то пошло не так. Попробуйте позже");
        });
});