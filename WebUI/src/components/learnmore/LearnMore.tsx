// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Flex, Button, Box, Card, Text, Label, Avatar, Image, Loader, Segment } from '@fluentui/react-northstar';
import { ChevronStartIcon } from '@fluentui/react-icons-northstar'
import { getChallengesbyid, getTotalAccepted, getPercentage, acceptChallenge, completeChallenge, leaveChallenge } from '../../services/learnmore-service';
import { UserPhoto } from 'model/User/UserPhoto';
import "./LearnMore.scss";

class LearnMore extends React.Component<any, any> {
    constructor(props: any) {
        super(props)
        this.state = {
            showDialog: false,
            ChallengesResults: {},
            CompletedPercentage: '0%',
            AcceptedCount: 0,
            isContentLoading: false,
            isAcceptLoading: false,
            isCompleteLoading: false
        }
    }

    componentDidMount() {
        if (this.props.challengeId) {
            const id = this.props.challengeId;
            this.getDetails(id);
        }
    }

    back() {

        if (this.props.backFunction) {
            this.props.backFunction();
        }
    }

    getDetails(id: any) {
        this.setState(({ isContentLoading: true }), () => {
            getChallengesbyid(id).then((res: any) => {
                this.setState({
                    ChallengesResults: res.data,
                    isContentLoading: false,
                    isAcceptLoading: false,
                    isCompleteLoading: false
                });
            }, err => { })
        })

        getPercentage(id).then((res: any) => {
            const doublePercentage = + res.data;
            const intPercentage = Math.round(doublePercentage);
            this.setState({ CompletedPercentage: intPercentage + '%' });
        }, err => { });

        getTotalAccepted(id).then((res: any) => {
            const totalAccepted = res.data;
            this.setState({ AcceptedCount: totalAccepted });
        }, err => { });
    }

    accept(id: any) {
        this.setState({ isAcceptLoading: true }, () => {
            acceptChallenge(id).then((res: any) => {
                const id = this.props.challengeId;
                this.getDetails(id);
            }, err => { })
        })
    }

    complete(id: any) {
        this.setState({ isCompleteLoading: true }, () => {
            completeChallenge(id).then((res: any) => {
                const id = this.props.challengeId;
                this.getDetails(id);
            }, err => { })
        })
    }

    leave(id: any) {
        leaveChallenge(id).then((res: any) => {
            const id = this.props.challengeId;
            this.getDetails(id);
        }, err => { })
    }

    openDialog() {
        this.setState({ showDialog: true });
    }

    closeDialog() {
        this.setState({ showDialog: false });
    }

    render() {
        return (
            this.state.isContentLoading ?
                <Loader />
                : <Flex wrap id="LearnMore">
                    <Segment className="navigation" styles={{ width: "100%" }} >
                        <Button className="back" icon={<ChevronStartIcon />} onClick={() => this.back()} content="Back" text />
                    </Segment>
                    {
                        this.state.ChallengesResults && this.state.ChallengesResults.items && !this.state.isContentLoading ?
                            this.state.ChallengesResults?.items.map((item: any) =>
                                <Flex wrap fill className="content">
                                    <Flex.Item grow size="60%">
                                        <Box className="challenge">
                                            <Text content="Challenge" />
                                            <Card className="challenge-card">
                                                <Card.Header>
                                                    <Flex>
                                                        <Flex className="challenge-thumbnail">
                                                            <Image src={item.thumbnail} alt='thumbnail' />
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
                                                        <Flex column className="challenge-header">
                                                            <Text content={item?.focusArea} />
                                                            <Text content={item?.title} as="h3" />
                                                            {
                                                                item.finalStatus === 0 ?
                                                                    item?.recurrence === 0 ?
                                                                        <Button onClick={() => this.complete(this.props.challengeId)} content="Complete Today" disabled={this.state.isCompleteLoading} />
                                                                        :
                                                                        <Button onClick={() => this.complete(this.props.challengeId)} content="Complete Challenge" disabled={this.state.isCompleteLoading} />
                                                                    : item.finalStatus === 1 ?
                                                                        null
                                                                        :
                                                                        <Button onClick={() => this.accept(this.props.challengeId)} content="Accept Challenge" disabled={this.state.isAcceptLoading} />
                                                            }

                                                        </Flex>
                                                        <Flex.Item push>
                                                            {
                                                                item?.challengeRecords && item.challengeRecords?.length > 0 ?
                                                                    item?.challengeRecords[0].status === 0 || (item?.recurrence === 0 && item?.challengeRecords[0].status !== 2) ?
                                                                        <Button className='leave-button' onClick={() => this.leave(this.props.challengeId)} content="Leave challenge" text />
                                                                        : null
                                                                    : null
                                                            }
                                                        </Flex.Item>
                                                    </Flex>
                                                </Card.Header>
                                                <Card.Body className="challenge-body">
                                                    {/* THIS FEATURE SHOULD BE DISABLED */}
                                                    {/* {
                                                    item.finalStatus == 0 || item.finalStatus == 1 ?
                                                    <Dialog
                                                        open={this.state.showDialog}
                                                        onOpen={(email: any) => this.openDialog()}
                                                        onCancel={() => this.closeDialog()}
                                                        onConfirm={() => this.closeDialog()}
                                                        header={
                                                            <div>                                                 
                                                                <div><Text weight="semibold" content="Sustainability Hub"/></div>
                                                                <div><Text size="small" weight="light" content="Challenge Someone"/></div>
                                                            </div>
                                                        }
                                                        content={ <ChallengeInvite challengeId={this.props.challengeId} closeEvent={ () => { this.closeDialog() }}/> }
                                                        trigger={ <Button content="Challenge Someone"/> }
                                                    />
                                                    : null
                                                } */}

                                                    <Text content="Description" weight="semibold" />
                                                    <Text className="description" content={item?.description} />
                                                </Card.Body>
                                            </Card>
                                        </Box>
                                    </Flex.Item>
                                    <Flex.Item grow size="40%">
                                        <Box>
                                            <Flex column className="challenge-info">
                                                <Text content={item?.title} weight="semibold" size="large" />
                                                <Box className="stats">
                                                    <Image src="bookmark.svg" />
                                                    <Text content={`${this.state.AcceptedCount}`} weight="bold" color="brand" />
                                                    <Text content={` have accepted this challenge`} color="brand" />
                                                </Box>
                                                <Box className="stats">
                                                    <Image src="trophy.svg" />
                                                    <Text content={`${this.state.CompletedPercentage}`} weight="bold" color="brand" />
                                                    <Text content={` have completed this challenge`} color="brand" />
                                                </Box>
                                            </Flex>
                                            {
                                                item?.relatedUsers && item?.relatedUsers.length > 0 ?
                                                    <Flex column className="related-users">
                                                        <Text content="People I work with in this challenge" weight="semibold" size="large" />
                                                        {
                                                            item?.relatedUsers.map((x: UserPhoto) => {
                                                                const workflow = x.status === 0 ? 'Accepted' : 'Completed';

                                                                return (
                                                                    <Flex className="" gap="gap.small" vAlign="center">
                                                                        {
                                                                            x.hasPhoto ?
                                                                                <Avatar name={x.firstName + " " + x.lastName} size='large' image={`data:image/jpeg;base64,${x.photo}`} />
                                                                                :
                                                                                <Avatar name={x.firstName + " " + x.lastName} size='large' />
                                                                        }
                                                                        <Flex column>
                                                                            <Text content={x.firstName + " " + x.lastName} size="small" weight="semibold" />
                                                                            <Text content={workflow} size="small" />
                                                                        </Flex>
                                                                    </Flex>
                                                                )
                                                            })
                                                        }
                                                    </Flex>
                                                    : null
                                            }
                                        </Box>
                                    </Flex.Item>
                                </Flex>
                            )
                            : null
                    }
                </Flex>
        )
    }
}

export default LearnMore;