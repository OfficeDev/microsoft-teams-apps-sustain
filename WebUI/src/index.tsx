import { axios } from 'common';
import { globalConfigUrl, globalConfig, defaultConfig } from 'config/config';
import { ReactElement } from 'react';
import ReactDOM from 'react-dom';
import SustainabilityApp from './app/SustainabilityApp';


const app: ReactElement = <SustainabilityApp />;

axios
    .get(globalConfigUrl)
    .then((response: any) => {
        globalConfig.set(response.data);
        console.debug("Global config fetched: ", response.data);
        return app;
    })
    .catch((e) => {
        if (process.env.NODE_ENV === "development") {
            // You cannot change the value of NODE_ENV. To test this if, change "development"
            console.warn(
                `Failed to load global configuration from '${globalConfigUrl}', using the default configuration instead:`,
                defaultConfig
            );
            globalConfig.set(defaultConfig);
            return app;
        } else {
            const errorMessage =
                "Error while fetching global config, the App wil not be rendered. (This is NOT a React error.)";
            console.error(
                errorMessage,
                `Have you provided the config file '${globalConfigUrl}'?`,
                e
            );
            return (
                <p style={{ color: "red", textAlign: "center" }}>{errorMessage}</p>
            );
        }
    })
    .then((reactElement: ReactElement) => {
        ReactDOM.render(reactElement, document.getElementById("root"));
    })
