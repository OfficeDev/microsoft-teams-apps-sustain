// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import Carousel from "./Carousel";
import Leaderboard from 'components/leaderboards/Leaderboard';
import HomeMonthlyChallenge from './Home-challenges';
import News from './News';
import Events from './Events';
import { Flex, Box } from '@fluentui/react-northstar';
import "./home.scss";
import LearnMore from 'components/learnmore/LearnMore';
import HomeDashboard from 'components/home/Home-dashboard';
import AdminNotice from 'components/common/AdminNotice';

class Home extends React.Component<{}, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0,
            isAdminView: false
        }
    }

    learnMoreGo(id: number) {
        if (id) {
            this.setState({
                ShouldShowLearnmore: true,
                SelectedChallengeId: id
            })
        }
    }

    learnMoreBack() {
        this.setState({
            ShouldShowLearnmore: false,
            SelectedChallengeId: 0
        });
    }

    viewAdminPage(isShown: boolean) {
        this.setState({ isAdminView: isShown });
    }

    render() {
        return (
            this.state.ShouldShowLearnmore ?
                <LearnMore challengeId={this.state.SelectedChallengeId} backFunction={() => this.learnMoreBack()} />
                :
                <div>
                    <AdminNotice />
                    <Flex wrap id="Home">
                        <Flex wrap fill>
                            <Flex.Item grow size="50%">
                                <Box>
                                    <HomeDashboard />
                                </Box>
                            </Flex.Item>
                            <Flex.Item grow size="50%">
                                <Box>
                                    <Carousel />
                                </Box>
                            </Flex.Item>
                        </Flex>
                        <Flex wrap fill>
                            <News />
                            <Events />
                        </Flex>
                        <Flex wrap fill styles={{ alignItems: "start" }}>
                            <Flex.Item grow size="65%">
                                <Box>
                                    <HomeMonthlyChallenge handleLearnMore={(id: number) => this.learnMoreGo(id)} />
                                </Box>
                            </Flex.Item>
                            <Flex.Item grow size="35%">
                                <Flex hAlign="center" styles={{ marginTop: "35px" }}>
                                    <Leaderboard fromHome='true' />
                                </Flex>
                            </Flex.Item>
                        </Flex>
                    </Flex>
                </div>
        )
    }
}

export default Home;