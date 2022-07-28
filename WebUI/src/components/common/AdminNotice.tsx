// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Alert, Button } from "@fluentui/react-northstar";
import { User } from "model/User/User";
import { UserRole } from "model/User/UserRole";
import React from "react";
import { Link } from "react-router-dom";
import { getMe } from "services/user-service";
import "./AdminNotice.scss";

interface AdminNoticeState {
    currentUser?: User
}

class AdminNotice extends React.Component<{}, AdminNoticeState> {

    constructor(props: any) {
        super(props)
        this.state = {
            currentUser: null
        };
    }

    componentDidMount(): void {
        this.getCurrentUser();
    }

    getCurrentUser() {
        this.setState({
            currentUser: null
        }, () => {
            getMe().then(res => {
                const user: User = res.data;
                this.setState({
                    currentUser: user
                });
            });
        })

    }

    render() {

        let isAdmin = false;

        if (this.state.currentUser && this.state.currentUser.userRoles) {
            this.state.currentUser.userRoles.forEach(
                (x: UserRole) => {
                    if (x.role.name.toLowerCase() === 'admin') {
                        isAdmin = true;
                    }
                }
            )
        }

        return (
            <>
                {
                    isAdmin ?
                        <Alert id="admin-notice"
                            content='This notification only appears for admins. Please click "Admin Settings" to manage configuration.'
                            actions={[
                                {
                                    content: (<Button primary><Link to="/admin-page">Admin Settings</Link></Button>),
                                    primary: true,
                                    key: 'adminSettings',
                                    text: true
                                }
                            ]}
                            dismissible />
                        : null
                }

            </>
        )
    }

}

export default AdminNotice;