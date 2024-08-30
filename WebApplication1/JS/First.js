var btn = document.getElementById("btn");
btn.addEventListener("click", Send);

async function Send() {
    var responce = await fetch("/api/user", {
        method: "POST",
        haders: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: document.getElementById("userName").value,
            age: document.getElementById("userAge").value
        })
    });

    var msg = await responce.json();
    document.getElementById("msg").innerText = msg.text;
}