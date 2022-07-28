// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useEffect, useState } from "react";
import { unstable_batchedUpdates as batchedUpdates } from "react-dom";
import { app } from "@microsoft/teams-js";
import { teamsDarkTheme, teamsHighContrastTheme, teamsTheme, ThemePrepared } from "@fluentui/react-northstar";

const getThemeFromUrl = (): string | undefined => {
    const urlParams = new URLSearchParams(window.location.search);
    const theme = urlParams.get("theme");
    return theme == null ? undefined : theme;
};

export function GetTheme(options?: { initialTheme?: string, setThemeHandler?: (theme?: string) => void }):
    [{
        theme: ThemePrepared,
        themeName: string,
        context?: app.Context
    },
        {
            setTheme: (theme: string | undefined) => void
        }] {
    const [theme, setTheme] = useState<ThemePrepared>(teamsTheme);
    const [themeName, setThemeName] = useState<string>("default");
    const [initialTheme] = useState<string | undefined>((options && options.initialTheme) ? options.initialTheme : getThemeFromUrl());
    const [context, setContext] = useState<app.Context | undefined>(undefined);

    const themeChangeHandler = (theme: string | undefined) => {
        // set the theme name
        setThemeName(theme || "default");
        switch (theme) {
            case "dark":
                setTheme(teamsDarkTheme);
                break;
            case "contrast":
                setTheme(teamsHighContrastTheme);
                break;
            case "default":
            default:
                setTheme(teamsTheme);
        }
    };

    const overrideThemeHandler = options?.setThemeHandler ? options.setThemeHandler : themeChangeHandler;

    useEffect(() => {
        overrideThemeHandler(initialTheme);

        app.initialize().then(() => {
            app.getContext().then(context => {
                batchedUpdates(() => {
                    setContext(context);
                });
                overrideThemeHandler(context.app.theme);
                app.registerOnThemeChangeHandler(overrideThemeHandler);
            }).catch();
        }).catch();
    }, [initialTheme, overrideThemeHandler]);

    return [{ theme, themeName, context }, { setTheme: overrideThemeHandler }];
}