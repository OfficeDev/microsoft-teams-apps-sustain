// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Form, FormInput, FormTextArea, FormDropdown, FormButton, Flex } from '@fluentui/react-northstar';
import { sendActivityNotification, getUsers } from 'services/challenge-invite-service';
import "./ChallengeInvite.scss";
import { FluentUIDropDownUser } from 'model/User/FluentUIDropDownUser';
import { UserPhoto } from 'model/User/UserPhoto';
import { NotificationRequest } from 'model/Notification/NotificationRequest';
import { globalConfig } from 'config/config';

class ChallengeInvite extends React.Component<any, any> {
    constructor(props: any) {
        super(props)
        this.state = {
            UsersToInvite: [],
            isDialogLoading: true,
            emailAddresses: [],
            isSubmitting: false
        }
    }

    componentDidMount() {
        this.getUsersToInvite();
    }

    back() {

        if (this.props.backFunction) {
            this.props.backFunction();
        }
    }

    getDetails() {
        this.setState(({
            isLoading: true,
            UsersToInvite: []
        }), () => {
            getUsers().then((res: any) => {
                var items = res.data.forEach((x: any) => {
                    return {
                        header: x.name,
                        image: x.photo,
                        content: x.emailAddress
                    }
                })
                this.setState({
                    isLoading: false,
                    UsersToInvite: items
                });

            }, err => {
                this.setState({
                    isLoading: false,
                    UsersToInvite: []
                });
            });
        }
        );
    }

    getUsersToInvite() {
        this.setState({
            isDialogLoading: true
        }, () => {
            getUsers().then((res: any) => {
                var items: FluentUIDropDownUser[] = [];
                res.data.forEach((x: UserPhoto) => {
                    items.push({
                        header: x.name,
                        content: x.emailAddress
                    })
                });
                this.setState({
                    UsersToInvite: items,
                    isDialogLoading: false
                });
            }, err => {
                this.setState({
                    UsersToInvite: [],
                    isDialogLoading: false

                });
            });
        })
    }

    onChangeHandler(_: any, event: any) {

    }

    submitForm = (event: any, props: any) => {

        const challengeId = this.props?.challengeId;
        const title = event.target.inviteTitle.value;
        const message = event.target.inviteMessage.value;
        const emailAddresses = this.state.emailAddresses.join(',');

        this.setState({ isSubmitting: true }, () => {
            const notifRequest: NotificationRequest = {
                AppId: globalConfig.get().REACT_APP_TEAMS_APP_ID,
                PageId: globalConfig.get().REACT_APP_CHALLENGE_PAGE_ID,
                Message: message,
                Recipients: emailAddresses,
                ChallengeId: challengeId,
                Title: title
            };

            sendActivityNotification(notifRequest).then(() => {
                this.setState({ isSubmitting: false });
                if (this.props.closeEvent) {
                    this.props.closeEvent();
                }
            }, err => {
                this.setState({ isSubmitting: false });
            });
        })
    }

    handleEmailChange = (event: any, props: any) => {
        const tempEmailAddresses: any = [];

        if (props.value) {
            props.value.forEach((e: any) => {
                if (e.content) {
                    tempEmailAddresses.push(e.content);
                }
            });
        }

        this.setState({ emailAddresses: tempEmailAddresses })
    }

    cancel() {
        if (this.props.closeEvent) {
            this.props.closeEvent();
        }
    }

    render() {
        return (
            <Form styles={{ padding: "10px" }} onSubmit={this.submitForm} >
                <FormDropdown onChange={this.handleEmailChange} fluid label="Who do you want to challenge?" items={this.state?.UsersToInvite} multiple />
                <FormInput name="inviteTitle" fluid label="Field Title" required />
                <FormTextArea fluid label="Message" name="inviteMessage" />

                <Flex gap="gap.small">
                    <FormButton content="Cancel" onClick={(() => { this.cancel() })} />
                    <FormButton disabled={this.state?.isSubmitting} loading={this.state?.isSubmitting} content="Submit" primary />
                </Flex>
            </Form>
        )
    }
}

export default ChallengeInvite