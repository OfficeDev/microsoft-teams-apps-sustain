// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Flex, Text, Card, Box, Avatar, Menu, Skeleton, Image, tabListBehavior } from '@fluentui/react-northstar';
import { getLeaderboard } from 'services/leaderboard-service';
import "./Leaderboard.scss";

class Leaderboard extends React.Component<any, any> {

    constructor(props: any) {
        super(props)

        this.state = {
            LeaderboardModel: {},
            buttonHighlight: true,
            fromHome: props.fromHome ? props.fromHome : false,
            leaderboardPeopleCount: props.fromHome ? 5 : 10,
            isLoading: true,
            error: true
        }

        this.ChangeLeaderboardDisplay = this.ChangeLeaderboardDisplay.bind(this)
    }

    componentDidMount() {
        this.ChangeLeaderboardDisplay(true);
    }

    ChangeLeaderboardDisplay(filteredByOrg: boolean) {
        this.setState({
            isLoading: true,
            LeaderboardModel: {},
            buttonHighlight: filteredByOrg,
            error: false
        }, () => {
            getLeaderboard(this.state.leaderboardPeopleCount, filteredByOrg).then((res: any) => {
                this.setState({
                    LeaderboardModel: res.data,
                    isLoading: false,
                    error: false
                });
            }, err => {
                this.setState({
                    LeaderboardModel: {},
                    isLoading: false,
                    error: true
                });
            })
        });
    }

    render() {
        return (
            <Card id="Leaderboard" elevated>
                <Card.Header>
                    <Box style={{ paddingBottom: 10 }}>
                        <img src="challenge-award.svg" alt="award" />
                        <Text content="Challenge" styles={{ "margin-left": "10px" }} size="small" />
                    </Box>
                    <Text content="Leaderboard" size="large" />
                </Card.Header>
                <Card.Body>
                    <Menu className="menu" defaultActiveIndex={0} underlined primary accessibility={tabListBehavior} items={[
                        {
                            className: "menu-item",
                            content: 'Org',
                            style: {
                                fontWeight: this.state.buttonHighlight ? 700 : 400
                            },
                            onClick: () => this.ChangeLeaderboardDisplay(true)
                        },
                        {
                            className: "menu-item",
                            content: 'People I Work With',
                            style: {
                                fontWeight: !this.state.buttonHighlight ? 700 : 400
                            },
                            onClick: () => this.ChangeLeaderboardDisplay(false)
                        }]}
                    />
                    {
                        this.state.error ?
                            <Flex column className="error-message" >
                                <Image src="error.svg" alt="" />
                                <Box>
                                    <Text content="Oh no! " size="small" weight="bold" />
                                    <Text content="Something is not connected. Please check back later." size="small" />
                                </Box>
                            </Flex>
                            : this.state.isLoading ?
                                <Skeleton animation="pulse">
                                    <Flex className="leaderboard-position" vAlign="center">
                                        <Skeleton.Avatar className="skeleton-avatar" size='large' />
                                        <Skeleton.Text size="small" />
                                    </Flex>
                                    <Flex className="leaderboard-position" vAlign="center">
                                        <Skeleton.Avatar className="skeleton-avatar" size='large' />
                                        <Skeleton.Text size="small" />
                                    </Flex>
                                    <Flex className="leaderboard-position" vAlign="center">
                                        <Skeleton.Avatar className="skeleton-avatar" size='large' />
                                        <Skeleton.Text size="small" />
                                    </Flex>
                                    <Flex className="leaderboard-position" vAlign="center">
                                        <Skeleton.Avatar className="skeleton-avatar" size='large' />
                                        <Skeleton.Text size="small" />
                                    </Flex>
                                    <Flex className="leaderboard-position" vAlign="center">
                                        <Skeleton.Avatar className="skeleton-avatar" size='large' />
                                        <Skeleton.Text size="small" />
                                    </Flex>
                                </Skeleton>
                                : this.state.LeaderboardModel && this.state.LeaderboardModel.items && this.state.LeaderboardModel.items.length === 0 ?
                                    <Flex column className="error-message" hAlign="center">
                                        <Image src="leaderboard-empty.svg" alt="" />
                                        <Box>
                                            <Text content="Accept a challenge " weight="semibold" color="brand" size="small" />
                                            <Text content="today to build your score" size="small" />
                                        </Box>
                                    </Flex>
                                    : this.state.LeaderboardModel.items.map((item: any) =>
                                        <Flex className="leaderboard-position" gap="gap.small" vAlign="center">
                                            {
                                                item.user.hasPhoto ?
                                                    <Avatar name={item.user.username} size='large' image={`data:image/jpeg;base64,${item.user.photo}`} /> :
                                                    <Avatar name={item.user.username} size='large' />
                                            }
                                            <Text truncated className="leaderboard-name" content={item.user.username} size="small" weight="semibold" />
                                            <Flex.Item push>
                                                <Box>
                                                    <img src="trophy.svg" alt="" /> &nbsp; {item.points}
                                                    <Text content={item.currentPoints} size="small" weight="semibold" />
                                                </Box>
                                            </Flex.Item>
                                        </Flex>
                                    )
                    }
                </Card.Body>
            </Card>
        )
    }
}

export default Leaderboard;