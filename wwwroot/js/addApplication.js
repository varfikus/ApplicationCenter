document.getElementById("applicationForm").addEventListener("submit", function (event) {
    event.preventDefault();

    const user = JSON.parse(localStorage.getItem("loggedInUser"));
    if (!user || !user.id) {
        alert("Вы не авторизованы");
        window.location.href = "login.html";
        return;
    }

    const formData = new FormData();
    formData.append("FullName", document.getElementById("fullName").value);
    formData.append("Email", document.getElementById("email").value);
    formData.append("Phone", document.getElementById("phone").value);
    formData.append("ServiceType", document.getElementById("serviceType").value);
    formData.append("Description", document.getElementById("description").value);
    formData.append("UserId", user.id);

    const files = document.getElementById("files").files;
    for (let i = 0; i < files.length; i++) {
        formData.append("Files", files[i]);
    }

    fetch("/api/Applications", {
        method: "POST",
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.status === "success") {
                alert("Заявка успешно отправлена!");
                window.location.href = "index.html";
            } else {
                alert("Ошибка при создании заявки.");
            }
        })
        .catch(error => {
            console.error("Ошибка:", error);
            alert("Не удалось отправить заявку.");
        });
});

document.getElementById('files').addEventListener('change', function () {
    const fileList = document.getElementById('fileList');
    fileList.innerHTML = ''; 

    if (this.files.length > 0) {
        const ul = document.createElement('ul');
        ul.classList.add('list-unstyled'); 

        Array.from(this.files).forEach((file, index) => {
            const li = document.createElement('li');
            li.classList.add('d-flex', 'align-items-center'); 

            const fileName = document.createElement('span');
            fileName.textContent = file.name;
            fileName.classList.add('me-2'); 

            const removeButton = document.createElement('button');
            removeButton.textContent = 'X'; 
            removeButton.classList.add('btn', 'btn-sm', 'btn-danger'); 
            removeButton.style.marginLeft = '10px';
            removeButton.addEventListener('click', () => {
                const newFileList = Array.from(this.files).filter((_, i) => i !== index);
                const dataTransfer = new DataTransfer(); 
                
                newFileList.forEach(file => dataTransfer.items.add(file));
                this.files = dataTransfer.files;

                li.remove(); 
            });

            li.appendChild(fileName);
            li.appendChild(removeButton);
            ul.appendChild(li);
        });

        fileList.appendChild(ul);
    }
});