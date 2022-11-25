import React from 'react'
import { Flex, Carousel, Text, Card, Box, Image, Skeleton } from '@fluentui/react-northstar'
import { pages } from "@microsoft/teams-js";
import { getDashboardDetails } from 'services/dashboard-service';
import { DashboardRankModel } from 'model/Dashboard/DashboardRank';
import { FluentUICarousel } from 'model/Carousel/FluentUICarousel';
import "./Home-dashboard.scss";
import { globalConfig } from 'config/config';

const PERSONAL_CARD_SKELETON = (
    <Skeleton animation="pulse">
        <div className="personal-card">
            <Flex>
                <Flex.Item size="90%">
                    <div>
                        <Skeleton.Text size='small' styles={{ width: "25%" }} />
                    </div>
                </Flex.Item>

                <Flex.Item size="10%">
                    <div>
                        <Skeleton.Text size='small' styles={{ width: "100%" }} />
                    </div>
                </Flex.Item>
            </Flex>
            <Flex vAlign="center">
                <Flex.Item size="35%">
                    <div>
                        <Skeleton.Text size='small' styles={{ width: "75%" }} />
                    </div>
                </Flex.Item>

                <Flex.Item size="65%">
                    <div>
                        <Skeleton.Text size='small' styles={{ width: "50%", marginTop: '5px' }} />
                        <Skeleton.Text size='small' styles={{ width: "50%", marginTop: '5px' }} />
                    </div>
                </Flex.Item>
            </Flex>

        </div>

        <div className="personal-bg-white">

        </div>

    </Skeleton>
);
const ERROR_CARD = (
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
)

class HomeDashboard extends React.Component<any, any> {

    constructor(props: any) {
        super(props)
        this.state = {
            DashboardDetails: {},
            isLoading: true
        }
    }

    componentDidMount() {
        this.getAll();
    }

    getAll() {
        this.setState({
            DashboardDetails: {},
            DashboardCarouselData: [],
            isLoading: true
        }, () => {

            getDashboardDetails().then((res: any) => {

                var carouselData: FluentUICarousel[] = [];
                var carouselContent: DashboardRankModel[] = [];
                var ctr: number = 1;
                var currentRankLabel = res.data.currentRankLabel;
                var nextRankLabel = res.data.nextRankLabel;

                res.data.dashboardRanks.forEach((y: DashboardRankModel) => {
                    carouselContent.push(y);
                    if (ctr % 5 == 0) {
                        carouselData.push(
                            {
                                "aria-label": (ctr / 5),
                                content: (
                                    <Flex>
                                        {
                                            carouselContent.map((x: DashboardRankModel) => {
                                                if (x.isActive) {
                                                    return (
                                                        <Flex.Item size="20%">
                                                            <Flex>
                                                                <Flex.Item>
                                                                    <div className="home-dashboard-badge">
                                                                        {
                                                                            currentRankLabel == x.label ?
                                                                                (
                                                                                    <div>
                                                                                        <Image className="home-dashboard-badge-icon-big" src={x.label.toLowerCase() + ".svg"} />
                                                                                    </div>
                                                                                )
                                                                                :
                                                                                (
                                                                                    <div>
                                                                                        <Image className="home-dashboard-badge-icon" src={x.label.toLowerCase() + ".svg"} />
                                                                                    </div>
                                                                                )
                                                                        }
                                                                        <div><Text content={x.label} weight="semibold" /></div>
                                                                        <div><Image className="trophy" src="trophy.svg" alt="" /><Text size="small" weight="semilight" content={x.score} /></div>
                                                                    </div>
                                                                </Flex.Item>
                                                            </Flex>
                                                        </Flex.Item>
                                                    )
                                                } else {
                                                    return (
                                                        <Flex.Item size="20%">
                                                            <Flex>
                                                                <Flex.Item>
                                                                    <div className="home-dashboard-badge">
                                                                        <div><Image className="home-dashboard-badge-icon" src={"disabled.svg"} /></div>
                                                                        <div><Text content={x.label} weight="semibold" /></div>
                                                                        <div><Image className="trophy" src="trophy.svg" alt="" /><Text size="small" weight="semilight" content={x.score} /></div>
                                                                    </div>
                                                                </Flex.Item>
                                                            </Flex>
                                                        </Flex.Item>
                                                    )
                                                }
                                            })
                                        }
                                    </Flex>
                                ),
                                id: (ctr / 5).toString(),
                                key: (ctr / 5).toString(),
                                styles: { width: "100%" }
                            }
                        );
                        carouselContent = [];
                    }
                    ctr++;
                });

                this.setState({
                    DashboardDetails: res.data,
                    DashboardCarouselData: carouselData,
                    isLoading: false,
                    isError: false
                });
            }, err => {
                this.setState({
                    DashboardDetails: {},
                    DashboardCarouselData: [],
                    isLoading: false,
                    isError: true
                });
                console.log(this.state);
            });
        });
    }

    navigateToChallengeTab() {
        pages.navigateToApp({ appId: globalConfig.get().REACT_APP_TEAMS_APP_ID, pageId: globalConfig.get().REACT_APP_CHALLENGE_PAGE_ID })
    }

    render() {
        return (
            <div id="Dashboard">
                {
                    this.state.isLoading ?
                        (PERSONAL_CARD_SKELETON)
                        :
                        (
                            <div>                    {
                                this.state.isError ?
                                    (ERROR_CARD)
                                    :
                                    (
                                        <div>
                                            <div className="personal-card">
                                                <Flex>
                                                    <Flex.Item size="90%">
                                                        <Text size="small" weight="regular" content={
                                                            "Hi " + this.state.DashboardDetails?.userName?.split(" ")[0] + ","
                                                        } />
                                                    </Flex.Item>

                                                    <Flex.Item size="10%">
                                                        <Text style={{ cursor: "pointer", float: "right", color: "#5B5FC7" }}
                                                            size="small"
                                                            weight="semibold"
                                                            content="Challenges"
                                                            onClick={() => this.navigateToChallengeTab()}
                                                        />
                                                    </Flex.Item>
                                                </Flex>

                                                <Flex vAlign="center">
                                                    <Flex.Item size="35%">
                                                        {
                                                            this.state.DashboardDetails.currentPoints >= this.state.DashboardDetails.minScore ?
                                                                (
                                                                    <Text size="small" weight="regular" content={
                                                                        <div>
                                                                            <span>Total Score <Image src="trophy.svg" alt="" /> </span>
                                                                            <Text size="small" weight="semibold" content={this.state.DashboardDetails.currentPoints} />
                                                                        </div>
                                                                    }
                                                                    />
                                                                )
                                                                :
                                                                (
                                                                    <Text size="small" content="Complete challenge to earn points" />
                                                                )
                                                        }
                                                    </Flex.Item>

                                                    <Flex.Item size="65%">
                                                        <Text size="small" weight="regular" content={
                                                            this.state.DashboardDetails.currentPoints >= this.state.DashboardDetails.maxScore ?
                                                                (

                                                                    <div>
                                                                        <div><span>You are a sustainability champion!</span></div>
                                                                        <div><span>but don't let this stop you from accepting new challenge.</span></div>
                                                                    </div>
                                                                )
                                                                :
                                                                (
                                                                    <div>
                                                                        <div><span>You need {this.state.DashboardDetails.remainingPoints} points to become {this.state.DashboardDetails.nextRankLabel}</span></div>
                                                                        <div><span>Accept a challenge today!</span></div>
                                                                    </div>
                                                                )
                                                        } />
                                                    </Flex.Item>
                                                </Flex>
                                                <Flex>
                                                    <Carousel
                                                        circular
                                                        paddlesPosition="outside"
                                                        id="Carousel"
                                                        navigation={{}}
                                                        paddleNext={{ hidden: this.state.DashboardDetails?.dashboardRanks?.length <= 5 }}
                                                        paddlePrevious={{ hidden: this.state.DashboardDetails?.dashboardRanks?.length <= 5 }}
                                                        items={this.state.DashboardCarouselData}
                                                        getItemPositionText={(index: number, size: any) => `${index + 1} of ${size}`}
                                                    />
                                                </Flex>

                                            </div>
                                            <div className="personal-bg-white"></div>

                                        </div>
                                    )
                            }
                            </div>
                        )
                }
            </div>
        )
    }
}

export default HomeDashboard