// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { FC } from "react";
import { useSearchParams } from "react-router-dom";
import styles from "./ErrorPage.module.scss";

const ErrorMessages = {
    generic: "Oops! An unexpected error seems to have occured. Try refreshing the page, or you can contact your administrator if the problem persists.",
    unauthorized: "Sorry, an error occurred while trying to access this service.",
    forbidden: "Sorry, you do not have permission to access this page. Please contact your administrator to be granted permission."
};

const ErrorPage: FC = () => {
    const [params] = useSearchParams();
    const code = params.get("code");

    let message = ErrorMessages.generic;
    switch (code) {
        case '401': message = ErrorMessages.unauthorized; break;
        case '403': message = ErrorMessages.forbidden; break;
    }

    return (
        <div className={styles.message}>
            {message}
        </div>
    );
};

export default ErrorPage;
