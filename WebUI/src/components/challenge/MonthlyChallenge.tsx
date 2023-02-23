// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Button, Flex, Card, Text, Label, Image, Box, Avatar, Loader } from '@fluentui/react-northstar';
import Leaderboard from 'components/leaderboards/Leaderboard';
import LearnMore from 'components/learnmore/LearnMore';
import { ChallengeRecord } from 'model/Challenge/ChallengeRecord';
import { ChallengesResults } from 'model/Challenge/ChallengeResult';
import { UserPhoto } from 'model/User/UserPhoto';
import { getChallenges } from 'services/challenge-service';
import { completeChallenge } from 'services/learnmore-service';
import { app } from "@microsoft/teams-js";
import "./MonthlyChallenge.scss";
import AdminNotice from 'components/common/AdminNotice';

class MonthlyChallenge extends React.Component<any, any> {

    public leaderboardChallenge:any;
    constructor(props: any) {
        super(props)
        this.state = {
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0,
            ChallengesResults: {},
            pageNumber: 1,
            maxPage: 6,
            status: -1,
            isLoading: true,
            loadingChallengeIds: [],
            pages: [],
            error: true
        }

        const ctx = app.getContext();

        ctx.then(res => {
            console.log(res);
        });

    }

    itemsAny?: ChallengesResults;

    componentDidMount() {

        this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
    }

    getAll(pageNumber: number, maxPage: number, selectedStatus: any) {
        this.setState({
            isLoading: true,
            ChallengesResults: {}
        }, () => {
            getChallenges(pageNumber, maxPage, selectedStatus)
                .then((res: any) => {
                    const maxPages = res.data.totalPages;
                    const pages = [];

                    for (let i = 1; i <= maxPages; i++) {
                        pages.push(i);
                    console.log("under get all refreshing leaderboard")
                    this.leaderboardChallenge.ChangeLeaderboardDisplay(true);
                    }

                    this.setState({
                        ChallengesResults: res.data,
                        isLoading: false,
                        pages: pages,
                        loadingChallengeIds: [],
                        error: false
                    });
                }, err => {
                    this.setState({
                        ChallengesResults: {},
                        isLoading: false,
                        loadingChallengeIds: [],
                        error: true
                    });
                })
        });
    }

    filterStatus(selectedStatus: number) {
        this.setState({
            status: selectedStatus,
            pageNumber: 1
        }, () => {
            this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
        });
    }

    goToPage(pageNumber: number) {
        if (pageNumber !== this.state.pageNumber) {
            const currentPage = pageNumber;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
            });
        }
    }

    nextPage() {
        if (this.state.ChallengesResults.hasNextPage) {
            const currentPage = this.state.pageNumber + 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
            });
        }
    }

    previousPage() {
        if (this.state.ChallengesResults.hasPreviousPage) {
            const currentPage = this.state.pageNumber - 1;

            this.setState({ pageNumber: currentPage }, () => {
                this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
            });
        }
    }

    concatText(text: string): string {
        if (text && text.length > 70) {
            return text.substring(0, 70) + '...';
        }

        return text;
    }

    learnMoreGo(id: number) {
        this.setState({
            ShouldShowLearnmore: true,
            SelectedChallengeId: id
        })
    }

    learnMoreBack() {
        this.setState({
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0
        })

        this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
    }

    complete(id: any) {
        const newState = this.state.loadingChallengeIds;
        newState.push(id);

        this.setState({ loadingChallengeIds: newState }, () => {
            completeChallenge(id).then((res: any) => {
                this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
                console.log("under complete refreshing leaderboard")
                    this.leaderboardChallenge.ChangeLeaderboardDisplay(true);
            }, err => { })
        });
    }

    isButtonLoading(id: any): boolean {
        if (id) {
            return this.state.loadingChallengeIds.indexOf(id) > -1;
        }

        return false;
    }

    render() {
        return (
            <>
                <AdminNotice />
                {
                    this.state.ShouldShowLearnmore ?
                        <LearnMore challengeId={this.state.SelectedChallengeId} backFunction={() => this.learnMoreBack()} />
                        : <Flex wrap fill id="MonthlyChallenges" styles={{ alignItems: "start" }}>
                            <Flex.Item grow size="70%">
                                <Box className="challenge-column">
                                    <Flex gap="gap.smaller" styles={{ paddingRight: "15px" }}>
                                        <Text content="Challenges" />
                                        <Flex.Item push>
                                            <Button onClick={() => this.filterStatus(-1)} primary={this.state.status === -1} secondary={this.state.status !== -1} content="All" loader="All" size="small" />
                                        </Flex.Item>
                                        <Button onClick={() => this.filterStatus(0)} primary={this.state.status === 0} secondary={this.state.status !== 0} content="Accepted" loader="Accepted" size="small" />
                                        <Button onClick={() => this.filterStatus(1)} primary={this.state.status === 1} secondary={this.state.status !== 1} content="Completed" loader="Completed" size="small" />
                                    </Flex>
                                    {
                                        this.state.isLoading ?
                                            <Loader />
                                            : this.state.error ?
                                                <Card fluid compact className='error-card'>
                                                    <Card.Preview fitted className='error-preview'>
                                                        <Image src="error.svg" alt="" />
                                                    </Card.Preview>
                                                    <Card.Header className='error-text'>
                                                        <Box>
                                                            <Text content="Oh no! " size="small" weight="bold" />
                                                            <Text content="Something is not connected. Please check back later." size="small" />
                                                        </Box>
                                                    </Card.Header>
                                                </Card>
                                                : this.state.ChallengesResults.totalCount <= 0 || !this.state.ChallengesResults.totalCount ?
                                                    <Card fluid className='error-card'>
                                                        <Card.Preview fitted className='error-preview'>
                                                            <Image src="monthly-challenge-empty.svg" alt="" />
                                                        </Card.Preview>
                                                        <Card.Header className='error-text'>
                                                            <Box>
                                                                <Text content="New challenges are coming in a jiffy. Check again tomorrow" size="small" />
                                                            </Box>
                                                        </Card.Header>
                                                    </Card>
                                                    : <Flex wrap>
                                                        {
                                                            this.state.ChallengesResults && this.state.ChallengesResults.items ?
                                                                this.state.ChallengesResults?.items.map((item: any) => {
                                                                    return (
                                                                        <Card className='challenge-card' elevated>
                                                                            <Card.Body fitted>
                                                                                <Flex column gap="gap.small">
                                                                                    <Image alt='challenge' className='thumbnail-image' src={item?.thumbnail} />
                                                                                    {
                                                                                        item.challengeRecords && item.challengeRecords?.length > 0 ? item.challengeRecords?.map((x: ChallengeRecord) => {
                                                                                            switch (x.status) {
                                                                                                // ACCEPTED
                                                                                                case 0: return (<Label className="status" circular content="Accepted" />)
                                                                                                // COMPLETED
                                                                                                case 1: return (<Label className="status" circular content="Completed" />)
                                                                                                // ABANDONED OR NOT TAKEN
                                                                                                default: return (null)
                                                                                            }
                                                                                        }) : null
                                                                                    }

                                                                                    {
                                                                                        item.recurrence === 0 ?
                                                                                            <Label icon="" className="point-container-daily challenge-point" circular>
                                                                                                <img src="trophy.svg" alt="" /> &nbsp; {item.points}/day
                                                                                            </Label>
                                                                                            :
                                                                                            <Label icon="" className="challenge-point" circular>
                                                                                                <img src="trophy.svg" alt="" /> &nbsp; {item.points}
                                                                                            </Label>
                                                                                    }
                                                                                    <Box styles={{ "margin-bottom": "5px" }}>
                                                                                        <Text content={item?.focusArea} size="smaller" />
                                                                                    </Box>
                                                                                    <Box styles={{ "margin-bottom": "5px" }}>
                                                                                        <Text className='title pointer' content={item?.title} size="large" weight="semibold" onClick={() => this.learnMoreGo(item.id)} />
                                                                                    </Box>
                                                                                    <Box styles={{ "margin-bottom": "15px" }}>
                                                                                        <Text content={this.concatText(item?.description)} size="small" />
                                                                                    </Box>

                                                                                    {/* RELATED USERS */}
                                                                                    {
                                                                                        item?.relatedUsers && item?.relatedUsers.length > 0 ?
                                                                                            item?.relatedUsers.map((x: UserPhoto) => {
                                                                                                if (x.hasPhoto) {

                                                                                                    return (<Avatar styles={{ "margin-bottom": "15px" }} name={x.name} size='large' image={`data:image/jpeg;base64,${x.photo}`} />)
                                                                                                } else {
                                                                                                    return (<Avatar styles={{ "margin-bottom": "15px" }} name={x.name} size='large' />)
                                                                                                }
                                                                                            }) : null
                                                                                    }

                                                                                    <Box>
                                                                                        {/* BUTTONS */}
                                                                                        {
                                                                                            item.challengeRecords && item.challengeRecords?.length > 0 ? item.challengeRecords?.map((x: ChallengeRecord) => {

                                                                                                switch (item.finalStatus) {
                                                                                                    // ACCEPTED
                                                                                                    case 0:
                                                                                                        return (
                                                                                                            item?.recurrence === 0 ?
                                                                                                                <Button onClick={() => this.complete(item.id)} content="Complete Today" disabled={this.isButtonLoading(item.id)} />
                                                                                                                : <Button onClick={() => this.complete(item.id)} content="Mark as Complete" disabled={this.isButtonLoading(item.id)} />
                                                                                                        )
                                                                                                    // COMPLETED
                                                                                                    case 1:
                                                                                                        return (null)
                                                                                                    // ABANDONED OR NOT TAKEN
                                                                                                    default:
                                                                                                        return (null)
                                                                                                }

                                                                                            }) : <Button onClick={() => this.learnMoreGo(item.id)} content="Learn More" />
                                                                                        }
                                                                                    </Box>
                                                                                </Flex>
                                                                            </Card.Body>
                                                                        </Card>
                                                                    )
                                                                })
                                                                : null
                                                        }
                                                    </Flex>
                                    }

                                    {/* PAGINATION  */}
                                    {
                                        !this.state.isLoading && this.state.ChallengesResults.totalCount > 0 &&
                                        <Flex className="pagination" style={{ margin: "20px", display: "flex", justifyContent: "center" }}>
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
                                </Box>
                            </Flex.Item>
                            <Flex.Item grow size="30%">
                                <Flex hAlign="center">
                                    <Leaderboard  ref={instances => { this.leaderboardChallenge = instances; }}/>
                                </Flex>
                            </Flex.Item>
                        </Flex>
                }
            </>
        )
    }
}

export default MonthlyChallenge