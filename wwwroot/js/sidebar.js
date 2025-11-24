document.addEventListener("DOMContentLoaded", () => {

    const sidebar = document.getElementById("sidebar");
    const sidebarSpacer = document.getElementById("sidebar-spacer");
    const icon = document.getElementById("toggleIcon");
    const toggleButton = document.getElementById("sidebarToggle");

    if (!sidebar || !sidebarSpacer || !icon || !toggleButton) {
        console.warn("FinTrack Sidebar: Elementos não encontrados.");
        return;
    }

    const applySidebarState = (collapsed) => {
        sidebar.classList.toggle("collapsed", collapsed);
        sidebarSpacer.classList.toggle("collapsed", collapsed);
        icon.className = collapsed
            ? "bi bi-chevron-double-right"
            : "bi bi-chevron-double-left";
    };

    const savedState = localStorage.getItem("sidebarCollapsed") === "true";
    applySidebarState(savedState);

    let isThrottled = false;
    toggleButton.addEventListener("click", () => {

        if (isThrottled) return;
        isThrottled = true;

        const collapsed = !sidebar.classList.contains("collapsed");

        applySidebarState(collapsed);
        localStorage.setItem("sidebarCollapsed", collapsed);

        setTimeout(() => {
            isThrottled = false;
        }, 250);
    });

    document.addEventListener("keydown", (e) => {

        if (!e.key || typeof e.key !== "string")
            return;

        if (e.key.toLowerCase() === "m") {
            toggleButton.click();
        }
    });

});
