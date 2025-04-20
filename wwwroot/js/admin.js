document.addEventListener("DOMContentLoaded", loadAll);
let currentAppId = null;

function loadAll() {
    fetch("/api/Admin/allapplications")
        .then(response => response.json())
        .then(data => renderTable(data))
        .catch(error => console.error("Ошибка загрузки всех заявок:", error));
}

function applyFilters() {
    const serviceType = document.getElementById("filterServiceType").value.trim();
    const status = document.getElementById("filterStatus").value;
    const fullName = document.getElementById("filterFullName").value.trim();
    const startDate = document.getElementById("filterStartDate").value;
    const endDate = document.getElementById("filterEndDate").value;

    let url = "/api/Admin/applications";
    const params = [];

    if (serviceType) params.push(`serviceType=${encodeURIComponent(serviceType)}`);
    if (status) params.push(`status=${encodeURIComponent(status)}`);
    if (fullName) params.push(`fullName=${encodeURIComponent(fullName)}`);
    if (startDate) params.push(`startDate=${encodeURIComponent(startDate)}`);
    if (endDate) params.push(`endDate=${encodeURIComponent(endDate)}`);

    if (params.length > 0) url += `?${params.join("&")}`;

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error("Ошибка при получении данных");
            }
            return response.json();
        })
        .then(data => renderTable(data))
        .catch(error => console.error("Ошибка применения фильтров:", error));
}

function deleteApplication(id) {
    if (!confirm("Вы уверены, что хотите удалить эту заявку?")) return;

    fetch(`/api/Admin/${id}`, { method: 'DELETE' })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => { throw err });
            }
            return response.json();
        })
        .then(data => {
            loadAll();
        })
        .catch(error => console.error("Ошибка удаления заявки:", error));
}

function showStatusDialog(id, currentStatus) {
    currentAppId = id;
    document.getElementById("newStatus").value = currentStatus;
    document.getElementById("statusModal").style.display = "block";
}

function closeStatusDialog() {
    document.getElementById("statusModal").style.display = "none";
    currentAppId = null;
}

function confirmStatusChange() {
    const newStatus = document.getElementById("newStatus").value;
    const formData = new FormData();

    const xmlContent = `<status>${newStatus}</status>`;
    const blob = new Blob([xmlContent], { type: "text/xml" });
    formData.append("status", newStatus);
    formData.append("xmlFile", blob, "status.xml");

    fetch(`/api/Admin/${currentAppId}`, {
        method: 'PATCH',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => { throw err });
            }
            return response.json();
        })
        .then(data => {
            console.log(data.message);
            closeStatusDialog();
            loadAll();
        })
        .catch(error => {
            console.error("Ошибка обновления статуса:", error);
            alert("Не удалось обновить статус");
        });
}

function renderTable(data) {
    const tbody = document.querySelector("#applicationsTable tbody");
    tbody.innerHTML = "";

    if (!data || data.length === 0) {
        const row = document.createElement("tr");
        row.innerHTML = "<td colspan='9'>Нет данных</td>";
        tbody.appendChild(row);
        return;
    }

    const statusDisplayNames = {
        "New": "Новая",
        "InProgress": "В работе",
        "Completed": "Завершена"
    };

    data.forEach(app => {
        const row = document.createElement("tr");

        row.innerHTML = `
        <td>${app.fullname ?? ""}</td>
        <td>${app.email ?? ""}</td>
        <td>${app.phone ?? ""}</td>
        <td class="text-break">${app.servicetype ?? ""}</td>
        <td class="text-break">${app.description ?? ""}</td>
        <td>${statusDisplayNames[app.status] ?? app.status}<br/><button onclick="showStatusDialog('${app.id}', '${app.status}')">Сменить</button></td>
        <td>${new Date(app.createdat).toLocaleString()}</td>
        <td>${(app.files || []).map(f => `<a href="${f}" target="_blank">Файл</a>`).join("<br>")}</td>
        <td>${app.xmlpath ? `<a href="${app.xmlpath}" target="_blank">XML</a>` : "—"}</td>
        <td><button onclick="deleteApplication('${app.id}')">Удалить</button></td>
        `;

        tbody.appendChild(row);
    });
}

function logout() {
    localStorage.removeItem("loggedInUser");
    window.location.href = "/index.html";
}