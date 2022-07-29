// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Flex, FlexItem, Image, Card, Box, Button, Text, Avatar, Loader, Label } from '@fluentui/react-northstar';
import { pages } from "@microsoft/teams-js";
import { ChallengeRecord } from 'model/Challenge/ChallengeRecord';
import { ChallengesResults } from 'model/Challenge/ChallengeResult';
import { UserPhoto } from 'model/User/UserPhoto';
import { getChallenges } from 'services/challenge-service';
import { completeChallenge } from 'services/learnmore-service';
import "./Home-challenges.scss";

class HomeMonthlyChallenge extends React.Component<any, any> {
    constructor(props: any) {
        super(props)
        this.state = {
            ChallengeCarouselData: [],
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0,
            ChallengesResults: {},
            pageNumber: 1,
            maxPage: 4,
            status: -1,
            isLoading: true,
            loadingChallengeIds: [],
            pages: [],
            error: true
        }
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
            getChallenges(pageNumber, maxPage, selectedStatus).then((res: any) => {
                const maxPages = res.data.totalPages;
                const pages = [];

                for (let i = 1; i <= maxPages; i++) {
                    pages.push(i);
                }

                this.setState({
                    ChallengesResults: res.data,
                    isLoading: false,
                    pages: pages,
                    error: false
                });

            }, err => {
                this.setState({
                    ChallengesResults: {},
                    isLoading: false,
                    error: true
                });

                console.log(this.state);
            })
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
        if (this.props.handleLearnMore) {
            this.props.handleLearnMore(id);
        }
    }

    learnMoreBack() {
        this.setState({
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0
        })
    }

    complete(id: any) {
        const newState = this.state.loadingChallengeIds;
        newState.push(id);

        this.setState({ loadingChallengeIds: newState }, () => {
            completeChallenge(id).then((res: any) => {
                this.getAll(this.state.pageNumber, this.state.maxPage, this.state.status);
            }, err => { })
        });
    }

    isButtonLoading(id: any): boolean {
        if (id) {
            return this.state.loadingChallengeIds.indexOf(id) > -1;
        }

        return false;
    }

    navigateToChallengeTab() {
        pages.navigateToApp({ appId: process.env.REACT_APP_TEAMS_APP_ID, pageId: process.env.REACT_APP_CHALLENGE_PAGE_ID })
    }

    render() {
        return (
            <Flex wrap id="HomeChallenges">
                <Box style={{ width: "100%", marginRight: "15px" }}>
                    <span>Challenges</span>
                    {
                        this.state.ChallengesResults && this.state.ChallengesResults.items ?
                            <Text style={{ cursor: "pointer", float: "right", color: "#5B5FC7" }}
                                weight="semibold"
                                content="See All"
                                onClick={() => this.navigateToChallengeTab()}
                            />
                            : null
                    }
                </Box>
                <Flex wrap fill>
                    {
                        this.state.isLoading ?
                            <Loader styles={{ width: "100%" }} />
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
                                    : this.state.ChallengesResults?.items.map((item: any) => {
                                        return (
                                            <Card compact fluid horizontal elevated className="challenge-card">
                                                <Card.Preview horizontal>
                                                    <Image alt='challenge' className='thumbnail-image' src={item?.thumbnail} />
                                                    <Flex>
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
                                                    </Flex>
                                                </Card.Preview>
                                                <Card.Column>
                                                    <Card.Header>
                                                        <Flex styles={{ "margin-bottom": "10px" }}>
                                                            <img src="challenge-award.svg" alt="award" />
                                                            <Text content="Challenge" styles={{ "margin-left": "10px" }} size="small" onClick={() => this.learnMoreGo(item.id)} />
                                                            {
                                                                item.challengeRecords && item.challengeRecords?.length > 0 ? item.challengeRecords?.map((x: ChallengeRecord) => {
                                                                    switch (x.status) {
                                                                        // ACCEPTED
                                                                        case 0: return (<FlexItem push><span className="status"> Accepted</span></FlexItem>)
                                                                        // COMPLETED
                                                                        case 1: return (<FlexItem push><span className="status"> Completed</span></FlexItem>)
                                                                        // ABANDONED OR NOT TAKEN
                                                                        default: return (null)
                                                                    }
                                                                }) : null
                                                            }
                                                        </Flex>
                                                        <Text className='title' content={item?.title} size="large" weight="semibold" onClick={() => this.learnMoreGo(item.id)} />
                                                    </Card.Header>
                                                    <Card.Body>
                                                        <Text styles={{ "margin-bottom": "15px" }} content={this.concatText(item?.description)} size="small" />
                                                        {
                                                            item?.relatedUsers && item?.relatedUsers.length > 0 ?
                                                                item?.relatedUsers.map((x: UserPhoto) => {
                                                                    if (x.hasPhoto) {
                                                                        return (<Avatar styles={{ "margin-bottom": "15px" }} name={x.name} size='large' image={`data:image/jpeg;base64,${x.photo}`} />)
                                                                    } else {
                                                                        return (<Avatar styles={{ "margin-bottom": "15px" }} name={x.name} size='large' />)
                                                                    }
                                                                })
                                                                : null
                                                        }
                                                    </Card.Body>
                                                    <Card.Footer fitted>
                                                        <Flex space="between" vAlign="center">
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
                                                                        default:
                                                                            return (null)
                                                                    }
                                                                }) : <Button onClick={() => this.learnMoreGo(item.id)} content="Learn More" />
                                                            }
                                                        </Flex>
                                                    </Card.Footer>
                                                </Card.Column>
                                            </Card>
                                        )
                                    })
                    }
                </Flex>
            </Flex>
        )
    }
}

export default HomeMonthlyChallenge;