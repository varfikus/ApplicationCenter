window.addEventListener("DOMContentLoaded", () => {
    const user = JSON.parse(localStorage.getItem("loggedInUser"));

    if (user && user.login) {
        document.getElementById("welcomeMessage").textContent = `Добро пожаловать, ${user.login}!`;
        document.getElementById("logoutButton").style.display = "inline-block";
        document.getElementById("createAppBtn").style.display = "inline-block";
        document.getElementById("myApps").style.display = "inline-block";
        document.getElementById("applicationsTable").style.display = "inline-block";
        document.getElementById("login").style.display = "none";
        document.getElementById("register").style.display = "none";

        fetch(`/api/Applications/${user.id}`)
            .then(response => response.json())
            .then(applications => {
                const tbody = document.querySelector("#applicationsTable tbody");
                tbody.innerHTML = "";

                if (applications.length === 0) {
                    const row = document.createElement("tr");
                    const cell = document.createElement("td");
                    cell.colSpan = 6;
                    cell.textContent = "Заявки не найдены.";
                    row.appendChild(cell);
                    tbody.appendChild(row);
                    return;
                }

                const statusDisplayNames = {
                    "New": "Новая",
                    "InProgress": "В работе",
                    "Completed": "Завершена"
                };

                applications.forEach(app => {
                    const row = document.createElement("tr");

                    row.innerHTML = `
                    <td>${new Date(app.createdat).toLocaleString()}</td>
                    <td class="text-break">${app.servicetype}</td>
                    <td class="text-break">${app.description}</td>
                    <td>${statusDisplayNames[app.status] ?? app.status}</td>
                    <td>${app.files && app.files.length > 0
                            ? app.files.map(f => `<a href="${f}" target="_blank">Файл</a>`).join(", ")
                            : "—"}
                    </td>
                    <td>
                    <a href="${app.xmlpath}" target="_blank">XML</a>
                    </td>
                    `;

                    tbody.appendChild(row);
                });
            })
            .catch(error => {
                console.error("Ошибка загрузки заявок:", error);
                alert("Не удалось загрузить заявки.");
            });
    } else {
        document.getElementById("welcomeMessage").textContent = "Добро пожаловать!";
        document.getElementById("logoutButton").style.display = "none";
        document.getElementById("createAppBtn").style.display = "none";
        document.getElementById("myApps").style.display = "none";
        document.getElementById("applicationsTable").style.display = "none";
        document.getElementById("login").style.display = "inline-block";
        document.getElementById("register").style.display = "inline-block";
    }
});

document.getElementById("createAppBtn").addEventListener("click", function () {
    const user = JSON.parse(localStorage.getItem("loggedInUser"));
    if (!user || !user.id) {
        alert("Сначала войдите в систему");
        window.location.href = "index.html";
        return;
    }
    window.location.href = "addApplication.html";
});

function logout() {
    localStorage.removeItem("loggedInUser");
    window.location.href = "index.html";
}