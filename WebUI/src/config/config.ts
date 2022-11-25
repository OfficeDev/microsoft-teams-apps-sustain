export interface DynamicConfig {
    REACT_APP_BASE_URL: string;
    REACT_APP_TEAMS_APP_ID: string;
    REACT_APP_CHALLENGE_PAGE_ID: string;
    REACT_APP_HOME_PAGE_ID: string;
}

export const defaultConfig: DynamicConfig = {
    REACT_APP_BASE_URL: "https://mssustain-api.azurewebsites.net",
    REACT_APP_TEAMS_APP_ID: "48fe6333-5b87-42a6-c800-2735fdd5e70c",
    REACT_APP_CHALLENGE_PAGE_ID: "56542934-1b74-409f-8d73-6b8e585043d8",
    REACT_APP_HOME_PAGE_ID: "fdc7f83b-acb4-495a-b024-025199d0ab67"
};

class GlobalConfig {
    config: DynamicConfig = defaultConfig;
    notDefinedYet = true;

    public get(): DynamicConfig {
        if (this.notDefinedYet) {
            throw new Error(
                "Global config has not been defined yet. Be sure to call the getter only after the config has been downloaded and set. Probable cause is accessing globalConfig in static context."
            );
        } else {
            return this.config;
        }
    }

    public set(value: DynamicConfig): void {
        if (this.notDefinedYet) {
            this.config = value;
            this.notDefinedYet = false;
        } else {
            throw new Error(
                "Global config has already been defined and now has been called second time. This is probably not intended."
            );
        }
    }
}

export const globalConfig = new GlobalConfig();

export const globalConfigUrl = "config.json";
