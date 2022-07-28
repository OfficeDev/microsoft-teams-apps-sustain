// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
    Button,
    Flex,
    Loader,
    Text,
    Input,
    EditIcon,
    Dialog,
    CloseIcon,
    AddIcon,
    FilterIcon,
    MenuButton,
    SearchIcon,
    Table
} from '@fluentui/react-northstar';
import { FluentUITableRows } from 'model/Common/FluenUITableRows';
import { User } from 'model/User/User';
import React from 'react';
import { getAllUsers } from 'services/user-service';
import './AdminUserManagement.scss';
import AdminUserManagementForm from 'components/admin/AdminUserManagementForm';

interface AdminUserManagementState {
    pages: number[],
    searchKeyword: string,
    role: string,
    showDialog: boolean,
    isLoading: boolean,
    selectedUserEmail: string,
    tableRows: any[],
    tableHeaders: any[],
    pageNumber: number,
    pageSize: number,
    userResults: any
}


class AdminUserManagement extends React.Component<any, AdminUserManagementState> {

    constructor(props: any) {
        super(props);
        this.state = {
            isLoading: true,
            pages: [],
            searchKeyword: '',
            role: '',
            selectedUserEmail: '',
            showDialog: false,
            tableHeaders: [
                'Email Address',
                'Role',
                ''
            ],
            tableRows: [],
            pageNumber: 1,
            pageSize: 10,
            userResults: {}
        }
    }

    mapToTableModel(users: User[]): FluentUITableRows[] {
        const result: FluentUITableRows[] = [];

        if (users && users.length > 0) {
            users.forEach((u: User) => {

                var roleName = '';

                if (u.userRoles && u.userRoles.length > 0) {
                    roleName = u.userRoles[0].role?.name;
                }

                const tableRow = {
                    key: u.id,
                    items: [
                        (
                            <Text content={u.email} />
                        ),
                        (
                            <Text content={roleName} />
                        ),
                        (
                            <Button onClick={() => { this.openDialog(u.email) }} icon={<EditIcon />} text />
                        )
                    ]
                }

                result.push(tableRow);
            });
        }

        return result;
    }

    componentDidMount(): void {
        this.getAllUsers();
    }

    getAllUsers() {
        this.setState({ isLoading: true }, () => {
            getAllUsers(this.state.pageNumber, this.state.pageSize, this.state.searchKeyword, this.state.role).then(res => {
                if (res.data && res.data.items && res.data.items.length > 0) {

                    const maxPages = res.data.totalPages;
                    const pages = [];

                    for (let i = 1; i <= maxPages; i++) {
                        pages.push(i);
                    }

                    const tableRowsResult = this.mapToTableModel(res.data.items);

                    this.setState({
                        isLoading: false,
                        tableRows: tableRowsResult,
                        userResults: res.data,
                        pages: pages
                    })
                }
            });

        })
    }

    filterUserRoles = (event: any, props: any) => {
        this.setState({ role: props.content === 'All' ? '' : props.content }, () => {
            this.getAllUsers();
        });

    }

    handleSearch = (event: any, props: any) => {
        const query = props.value;
        this.setState({ searchKeyword: query, pageNumber: 1 }, () => { this.getAllUsers(); })
    }

    debounce = (func: any) => {
        let timer: any;
        return (...args: any) => {
            const context = this;
            if (timer) clearTimeout(timer);
            timer = setTimeout(() => {
                timer = null;
                func.apply(context, args);
            }, 500);
        };
    };

    openDialog(userEmail: string) {
        this.setState(
            { selectedUserEmail: userEmail },  // set selected link first
            () => this.setState({ showDialog: true }) // set showDialog to open
        )
    }

    closeDialog() {
        this.setState({ showDialog: false })
    }

    goToPage(pageNumber: number) {
        if (pageNumber !== this.state.pageNumber) {
            const currentPage = pageNumber;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllUsers();
            });
        }
    }

    nextPage() {
        if (this.state.userResults.hasNextPage) {
            const currentPage = this.state.pageNumber + 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllUsers();
            });
        }
    }

    previousPage() {
        if (this.state.userResults.hasPreviousPage) {
            const currentPage = this.state.pageNumber - 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllUsers();
            });
        }
    }

    render() {
        return (
            <>
                <Dialog
                    styles={{ maxWidth: "35%!important" }}
                    open={this.state.showDialog}
                    onOpen={(email: any) => this.openDialog(email)}
                    onCancel={() => this.closeDialog()}
                    onConfirm={() => this.closeDialog()}
                    header={this.state.selectedUserEmail.length > 0 ? "Edit User" : "Add User"}
                    content={
                        <AdminUserManagementForm email={this.state.selectedUserEmail} closeEvent={() => { this.closeDialog() }} submitEvent={() => { this.getAllUsers() }} />
                    }
                    headerAction={{
                        icon: <CloseIcon />,
                        title: 'Close',
                        onClick: () => this.closeDialog(),
                    }}
                />

                <Flex wrap id='AdminUserManagement'>

                    <Flex.Item size='70%'>
                        <Flex className='buttons-container'>
                            <Button content='New User' icon={<AddIcon />} primary={true} onClick={() => { this.openDialog(""); }} />
                        </Flex>
                    </Flex.Item>

                    <Flex.Item size='30%'>
                        <Flex className='buttons-container'>
                            <MenuButton
                                trigger={<Button icon={<FilterIcon />} content='Filter' text />}
                                menu={['All', 'Admin', 'User']}
                                onMenuItemClick={this.filterUserRoles}
                            />
                            <Input placeholder='Find' icon={<SearchIcon />} onChange={this.debounce(this.handleSearch)} fluid clearable />
                        </Flex>
                    </Flex.Item>

                    <Flex.Item size='100%'>
                        {
                            this.state.isLoading ? <Loader /> :
                                <Flex className='users-table-container'>
                                    <Table
                                        className='users-table'
                                        header={this.state.tableHeaders}
                                        rows={this.state.tableRows}
                                        aria-label="Static table" />
                                </Flex>
                        }
                    </Flex.Item>

                    {
                        !this.state.isLoading && this.state.userResults.totalCount > 0 &&
                        <Flex className="pagination" style={{ margin: "20px", display: "flex", justifyContent: "center", width: "100%" }}>
                            <Button content="Prev" iconOnly title="Prev" onClick={() => this.previousPage()} />
                            {
                                this.state.pages.map((x: any) => {
                                    return (<Button content={x.toString()} primary={x === this.state.pageNumber}
                                        iconOnly title={x.toString()} onClick={() => this.goToPage(x)} />)
                                })
                            }
                            <Button content="Next" iconOnly title="Next" onClick={() => this.nextPage()} />
                        </Flex>
                    }

                </Flex>
            </>
        )
    }
}

export default AdminUserManagement;