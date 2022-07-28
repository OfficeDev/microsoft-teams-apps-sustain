// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
    Form,
    FormInput,
    FormButton,
    Flex,
    FormDropdown,
    Loader,
    Text,
    Dropdown
} from '@fluentui/react-northstar';
import { User } from 'model/User/User';
import React from 'react';
import { addUser, editUser, getUser, searchUsers } from 'services/user-service';
import './AdminUserManagementForm.scss';

interface AdminUserManagementFormState {
    user?: User;
    role: string;
    isLoading: boolean;
    isSaving: boolean;
    selectedFile?: File;
    errorMessage?: string;
    searchedUsers?: any[];
    selectedEmails?: string[];
}

class AdminUserManagementForm extends React.Component<any, AdminUserManagementFormState> {

    constructor(props: any) {
        super(props)

        if (props && props.email) {
            this.state = {
                user: {
                    email: props.email,
                    userRoles: []
                },
                isLoading: true,
                isSaving: false,
                errorMessage: '',
                role: 'User',
                searchedUsers: [],
                selectedEmails: []
            }
        } else {
            this.state = {
                user: {
                    email: '',
                    userRoles: []
                },
                isLoading: false,
                isSaving: false,
                errorMessage: '',
                role: 'User',
                searchedUsers: [],
                selectedEmails: []
            }
        }

    }

    componentDidMount(): void {

        if (this.state.user.email && this.state.user.email.length > 0) {
            this.getValue(this.state.user.email);
        }
    }

    getValue(email: string) {

        this.setState({ isLoading: true }, () => {
            getUser(email,).then(
                (res: any) => {
                    const data: User = res.data;
                    if (data) {
                        const hasRole = data.userRoles && data.userRoles.length > 0;
                        this.setState({
                            isLoading: false,
                            user: data,
                            role: hasRole ? data.userRoles[0].role.name : 'User'
                        })

                    }
                }
            )

        });

    }

    onRoleChanged = (e: any, props: any) => {
        this.setState({
            role: props.value
        })
    }

    submitForm = (e: any, props: any) => {
        const role = this.state.role;

        this.setState({ isSaving: true, errorMessage: '' }, () => {

            if (this.state.user.email && this.state.user.email.length > 0) {

                editUser(this.state.user.email, role).then(res => {
                    this.setState({ isSaving: false })
                    this.props.closeEvent();
                    this.props.submitEvent();

                }, err => {
                    this.setState({ isSaving: false });
                })
            } else {
                addUser(this.state.selectedEmails, this.state.role).then(res => {
                    this.setState({ isSaving: false });
                    this.props.closeEvent();
                    this.props.submitEvent();
                }, err => {
                    this.setState({ isSaving: false, errorMessage: 'One or more users has been already added to the database.' });
                })
            }
        });
    }

    cancel() {
        if (this.props.closeEvent) {
            this.props.closeEvent();
        }
    }

    handlePeoplePickerChange = (event: any, props: any) => {
        const query = props.searchQuery;
        this.searchUserByName(query);
    }

    handlePeoplePickerOnSelectChange = (event: any, props: any) => {
        const emailAddresses: string[] = [];

        if (props.value && props.value.length > 0) {
            props.value.forEach((element: any) => {
                if (element && element.content) {
                    emailAddresses.push(element.content);
                }
            });
        }

        this.setState({ selectedEmails: emailAddresses })
    }

    searchUserByName(query: string) {
        searchUsers(query).then(res => {
            const users: any = [];

            if (res.data && res.data.length > 0) {
                res.data.forEach((u: any) => {
                    users.push({
                        header: u.firstName,
                        content: u.emailAddress
                    });
                });
            }

            if (users && users.length > 0) {
                this.setState({
                    searchedUsers: users
                })
            }

        }, err => {

        });
    }

    render() {
        const isEdit = this.state.user?.email && this.state.user?.email.length > 0;
        const isInvalid = !isEdit && this.state.selectedEmails.length <= 0;
        return (
            this.state.isLoading ?
                <Loader /> :
                <Flex wrap id="admin-usermanagement-form" >
                    <Form
                        onSubmit={this.submitForm}
                    >
                        {/* EMAIL */}
                        {
                            this.state.user?.email && this.state.user?.email.length > 0 ?
                                // EDIT = SINGLE
                                <FormInput className="form-margin"
                                    fluid
                                    label="Email Address"
                                    name="email"
                                    id="email"
                                    disabled={this.state.user.email ? true : false}
                                    showSuccessIndicator={false}
                                    defaultValue={this.state.user?.email}
                                    required
                                /> :
                                // ADD = MULTIPLE
                                <Dropdown
                                    multiple
                                    search
                                    items={this.state.searchedUsers}
                                    placeholder="Start typing a name"
                                    // getA11ySelectionMessage={getA11ySelectionMessage}
                                    noResultsMessage="We couldn't find any matches."
                                    a11ySelectedItemsMessage="Press Delete or Backspace to remove"
                                    onSearchQueryChange={this.handlePeoplePickerChange}
                                    onChange={this.handlePeoplePickerOnSelectChange}
                                />
                        }


                        {/* ROLE  */}
                        <FormDropdown
                            className="form-margin"
                            onChange={this.onRoleChanged}
                            label="Role"
                            items={['User', 'Admin']}
                            defaultValue={this.state.user.userRoles && this.state.user.userRoles.length > 0 ? this.state.user.userRoles[0].role.name : 'User'}
                        />

                        {
                            this.state.errorMessage && this.state.errorMessage.length > 0 ? <Text error={true} content={this.state.errorMessage} /> : null
                        }

                        {/* BUTTONS */}
                        <Flex gap="gap.small">
                            <FormButton content="Cancel" onClick={() => { this.cancel() }} />
                            <FormButton disabled={this.state.isSaving || isInvalid} loading={this.state.isSaving} primary content="Save" />
                        </Flex>

                    </Form>

                </Flex>
        )
    }
}

export default AdminUserManagementForm;