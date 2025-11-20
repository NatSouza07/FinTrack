document.getElementById("sidebarToggle").onclick = () => {
    const sidebar = document.getElementById("sidebar");
    const main = document.getElementById("mainContent");
    const icon = document.getElementById("toggleIcon");

    sidebar.classList.toggle("collapsed");
    main.classList.toggle("collapsed");

    const collapsed = sidebar.classList.contains("collapsed");

    icon.className = collapsed
        ? "bi bi-chevron-double-right"
        : "bi bi-chevron-double-left";
};
