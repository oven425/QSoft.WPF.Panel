import { useEffect, useState } from "react";

export type Theme = "dark" | "light" | "system";
const useThemeMode = (theme:Theme='system') => {
    const [themeMode, setThemeMode] = useState<Theme>(theme);
    const [isDarkMode, setIsDarkMode] = useState<boolean>(false);

    
    useEffect(() => {
        if (themeMode === 'dark') {
            setIsDarkMode(true);
        }
        else if (themeMode === 'system') {
            let systemdark = window.matchMedia("(prefers-color-scheme: dark)").matches;
            setIsDarkMode(systemdark);
        }
        else {
            setIsDarkMode(false)
        }
    }, [themeMode]);

    useEffect(() => {
        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
        const handleChange = (e: MediaQueryListEvent) => {
            if (themeMode === 'system') {
                const isDark = e.matches;
                setIsDarkMode(isDark);
            }
        };
        mediaQuery.addEventListener('change', handleChange);
        return () => {
            mediaQuery.removeEventListener('change', handleChange);
        };
    }, []);

    useEffect(() => {
        document.documentElement.classList.toggle('dark', isDarkMode);
    }, [isDarkMode])

    return [themeMode, setThemeMode];
}
export default useThemeMode;