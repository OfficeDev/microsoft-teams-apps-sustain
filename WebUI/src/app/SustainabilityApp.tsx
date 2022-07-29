// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { BrowserRouter, Outlet, Route, Routes } from 'react-router-dom';
import { Provider, mergeThemes } from "@fluentui/react-northstar";
import { DashboardPage, ErrorPage, YammerPage, ChallengesPage, AdminPage } from 'pages';
import { GetTheme } from "common/MSTeams";

import 'bootstrap/dist/css/bootstrap.min.css';

const Chrome = () => {
    const [{ theme, themeName }] = GetTheme({});

    // invert background and card color on default theme
    const customTheme = {
        siteVariables: {
            bodyBackground: '#f5f5f5'
        },
        componentVariables: {
            Card: {
                backgroundColor: "#ffffff"
            }
        }
    }

    return (
        <Provider theme={themeName === "default" ? mergeThemes(theme, customTheme) : theme} styles={{ "min-height": "100vh" }}>
            <Outlet />
        </Provider>
    )
}

const SustainabilityApp = () =>
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<Chrome />}>

                <Route index element={<DashboardPage />} />
                <Route path="/challenges" element={<ChallengesPage />} />
                <Route path="/yammer" element={<YammerPage />} />
                <Route path="/error" element={<ErrorPage />} />
                <Route path="/admin-page" element={<AdminPage />} />
            </Route>
        </Routes>
    </BrowserRouter>

export default SustainabilityApp;