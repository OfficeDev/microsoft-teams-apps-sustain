// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Flex, Image, Button, Input, AddIcon, Table, AcceptIcon, Text, Box, Dialog, CloseIcon, Loader, FilterIcon, MenuButton, SearchIcon } from '@fluentui/react-northstar';
import { Challenge } from 'model/Challenge/Challenge';
import { FluentUITableRows } from 'model/Common/FluenUITableRows';
import React from 'react'
import { getChallengesManagement } from 'services/challenge-service';
import './AdminChallenge.scss'
import AdminChallengeForm from './AdminChallengeForm';

class AdminChallenge extends React.Component<{}, any> {
    constructor(props: any) {
        super(props);

        this.state = {
            pages: [],
            challengesResult: {},
            pageNumber: 1,
            pageSize: 10,
            title: '',
            description: '',
            isLoading: true,
            selectedChallengeId: 0,
            tableRows: [],
            tableHeaders: [
                'Challenge title',
                'Points',
                'Active Until',
                'Focus Area',
                'Additional Resources',
                ''
            ],
            ShowDialog: false,
            isArchived: false,
            keyword: ''
        }
    }

    mapToTableModel(challenges: Challenge[]): FluentUITableRows[] {
        const result: FluentUITableRows[] = [];

        if (challenges && challenges.length > 0) {
            challenges.forEach((x: Challenge) => {
                result.push({
                    key: x.id,
                    items: [
                        (
                            <Box className="title-container" onClick={() => { return this.openDialog(x.id); }}>
                                <Image className='thumbnail' src={x.thumbnail} />
                                <Text content={this.concatText(x.title)} />
                            </Box>
                        ),
                        x.points,
                        (
                            new Intl.DateTimeFormat('en-US').format(new Date(x.activeUntil))
                        ),
                        x.focusArea,
                        (x.additionalResources ? <AcceptIcon /> : <Text content="" />),
                        (
                            x.isPinned ? <Button icon={<Image src="pin.svg" />} text /> : <Text content="" />
                        ),
                    ]
                })
            });
        }

        return result;
    }

    componentDidMount(): void {
        this.getAllChallenges();
    }

    filterChallanges = (event: any, props: any) => {
        const isArchivedVal = props.content === "Active" ? false : true;
        this.setState({ isArchived: isArchivedVal });
        this.getAllChallenges();
    }

    handleSearch = (event: any, props: any) => {
        const query = props.value;
        this.setState({ keyword: query }, () => { this.getAllChallenges() })
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

    getAllChallenges() {

        this.setState({ isLoading: true }, () => {
            getChallengesManagement(
                this.state.pageNumber,
                this.state.isArchived,
                this.state.pageSize,
                this.state.keyword,
            ).then(res => {

                const maxPages = res.data.totalPages;
                const pages = [];

                for (let i = 1; i <= maxPages; i++) {
                    pages.push(i);
                }

                const latestTableRows = this.mapToTableModel(res.data.items);

                this.setState({
                    challengesResult: res.data,
                    tableRows: latestTableRows,
                    isLoading: false,
                    pages: pages
                })
            }, err => {
                this.setState({
                    challengesResult: {},
                    isLoading: false
                })
            });
        })

    }

    concatText(text: string): string {
        if (text && text.length > 15) {
            return text.substring(0, 15) + '...';
        }

        return text;
    }

    openDialog(id: number) {
        this.setState(
            { selectedChallengeId: id },  // set selected link first
            () => this.setState({ ShowDialog: true }) // set showDialog to open
        )
    }

    closeDialog() {
        this.setState({ ShowDialog: false })
    }

    goToPage(pageNumber: number) {
        if (pageNumber !== this.state.pageNumber) {
            const currentPage = pageNumber;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllChallenges();
            });
        }
    }

    nextPage() {
        if (this.state.challengesResult.hasNextPage) {
            const currentPage = this.state.pageNumber + 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllChallenges();
            });
        }
    }

    previousPage() {
        if (this.state.challengesResult.hasPreviousPage) {
            const currentPage = this.state.pageNumber - 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAllChallenges();
            });
        }
    }

    render(): React.ReactNode {
        return (
            <>

                <Dialog
                    styles={{ maxWidth: "35%!important" }}
                    open={this.state.ShowDialog}
                    onOpen={(id: any) => this.openDialog(id)}
                    onCancel={() => this.closeDialog()}
                    onConfirm={() => this.closeDialog()}
                    header={this.state.selectedChallengeId > 0 ? "Edit Challenge" : "Add Challenge"}
                    content={
                        <AdminChallengeForm challengeId={this.state.selectedChallengeId} closeEvent={() => { this.closeDialog() }} submitEvent={() => { this.getAllChallenges() }} />
                    }
                    headerAction={{
                        icon: <CloseIcon />,
                        title: 'Close',
                        onClick: () => this.closeDialog(),
                    }}
                />

                <Flex wrap id='AdminChallenge'>

                    <Flex.Item size='70%'>
                        <Flex className='buttons-container'>
                            <Button content='New Challenge' icon={<AddIcon />} primary={true} onClick={() => { this.openDialog(0); }} />
                        </Flex>
                    </Flex.Item>

                    <Flex.Item size='30%'>
                        <Flex className='buttons-container'>
                            <MenuButton
                                trigger={<Button icon={<FilterIcon />} content='Filter' text />}
                                menu={['Active', 'Archived']}
                                onMenuItemClick={this.filterChallanges}
                            />
                            <Input placeholder='Find' icon={<SearchIcon />} onChange={this.debounce(this.handleSearch)} fluid clearable />
                        </Flex>
                    </Flex.Item>

                    <Flex.Item size='100%'>
                        {
                            this.state.isLoading ? <Loader /> :
                                <Flex className='challenges-table-container'>
                                    <Table
                                        className='challenges-table'
                                        header={this.state.tableHeaders}
                                        rows={this.state.tableRows}
                                        aria-label="Static table" />
                                </Flex>
                        }
                    </Flex.Item>

                    {
                        !this.state.isLoading && this.state.challengesResult.totalCount > 0 &&
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

export default AdminChallenge;