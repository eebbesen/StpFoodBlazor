(() => {
    const storageKey = "stpfood-theme";
    const darkTheme = "dark";
    const lightTheme = "light";

    const prefersDarkScheme = () =>
        window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches;

    const getStoredTheme = () => {
        try {
            return window.localStorage.getItem(storageKey);
        } catch {
            return null;
        }
    };

    const setStoredTheme = (theme) => {
        try {
            window.localStorage.setItem(storageKey, theme);
        } catch {
            // no-op when storage is unavailable
        }
    };

    const resolveInitialTheme = () => {
        const storedTheme = getStoredTheme();
        if (storedTheme === darkTheme || storedTheme === lightTheme) {
            return storedTheme;
        }

        return prefersDarkScheme() ? darkTheme : lightTheme;
    };

    const applyTheme = (theme) => {
        const darkMode = theme === darkTheme;
        document.body.classList.toggle("dark-mode", darkMode);
        document.documentElement.style.colorScheme = darkMode ? darkTheme : lightTheme;
        return darkMode;
    };

    window.themeManager = {
        initialize() {
            const theme = resolveInitialTheme();
            return applyTheme(theme);
        },
        toggle() {
            const darkModeEnabled = !document.body.classList.contains("dark-mode");
            const nextTheme = darkModeEnabled ? darkTheme : lightTheme;
            setStoredTheme(nextTheme);
            return applyTheme(nextTheme);
        }
    };
})();
