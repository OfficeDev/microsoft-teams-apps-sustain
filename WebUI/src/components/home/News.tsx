// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Button, Card, Flex, Text, Image, Box, Skeleton } from '@fluentui/react-northstar';
import { getNews } from 'services/sharepoint-service';
import "./News.scss";

class News extends React.Component<{}, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            News: [],
            isLoading: true,
            characterLimit: 49,
            columnWidth: 3,
            oneCard: false,
            selectedValueLink: '',
            selectedValueLogo: ''
        }
    }

    componentDidMount() {
        this.getSPNews();
    }

    getSPNews() {
        this.setState({
            News: [],
            isLoading: true
        }, () => {
            getNews().then((res: any) => {
                this.setState({
                    News: res.data,
                    isLoading: false,
                    error: false
                });
                switch (res.data.length) {
                    case 3:
                        this.setState({ characterLimit: 60, columnWidth: 4, oneCard: false });
                        break;
                    case 2:
                        this.setState({ characterLimit: 90, columnWidth: 6, oneCard: false });
                        break;
                    case 1:
                        this.setState({ characterLimit: 135, columnWidth: 12, oneCard: true });
                        break;
                    default:
                        this.setState({ characterLimit: 39, columnWidth: 3, oneCard: false });
                        break;
                }
            }, err => {
                this.setState({
                    News: [],
                    isLoading: false,
                    error: true
                });
            })
        });


    }

    concatText(text: string): string {
        if (text && text.length > this.state.characterLimit) {
            return text.substring(0, this.state.characterLimit) + '...';
        }

        return text;
    }

    openInWindow(url: string) {
        window.open(url)
    }

    openDialog(link: string, logo: string) {
        this.setState(
            { selectedValueLink: link, selectedValueLogo: logo },  // set selected link first
            () => this.setState({ ShowDialog: true }) // set showDialog to open
        )
    }

    closeDialog() {
        this.setState({ ShowDialog: false })
    }

    render() {
        return (
            <>
                {
                    this.state.isLoading || this.state.error || (this.state.News && this.state.News.length > 0) ?
                        <Flex.Item grow size="70%">
                            <Box>
                                <Flex column id="News">
                                    <Flex>
                                        <span>News</span>
                                        {
                                            <Flex.Item push>
                                                <Text styles={{ cursor: "pointer", float: "right", color: "#5B5FC7" }} weight="semibold" content="See All" onClick={() => this.openInWindow(this.state.News[0].seeAllUrl)} />
                                            </Flex.Item>
                                        }
                                    </Flex>
                                    <Flex className="news-list" wrap style={{ marginBottom: "15px" }}>
                                        {
                                            this.state.isLoading ?
                                                Array(4).fill(1, 0).map(() =>
                                                    <Card compact fluid className="news-card">
                                                        <Skeleton animation="pulse">
                                                            <Card.Preview>
                                                                <Skeleton.Shape className="news-thumbnail" />
                                                            </Card.Preview>
                                                            <Card.Header fitted className="news-header">
                                                                <Flex column gap="gap.small" padding="padding.medium">
                                                                    <Skeleton.Text size="small" />
                                                                    <Skeleton.Text size="small" />
                                                                </Flex>
                                                            </Card.Header>
                                                            <Card.Footer className="news-footer">
                                                                <Flex>
                                                                    <Skeleton.Text size="small" />
                                                                </Flex>
                                                            </Card.Footer>
                                                        </Skeleton>
                                                    </Card>
                                                )
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
                                                    : this.state.News.map((item: any) =>
                                                        <Card compact className="news-card">
                                                            <Card.Preview>
                                                                <Image className="news-thumbnail" src={item?.thumbnailUrl} />
                                                            </Card.Preview>
                                                            <Card.Header fitted className="news-header">
                                                                <Flex column padding="padding.medium">
                                                                    <Text size="small" weight="semibold" content={this.concatText(item?.title)} />
                                                                    {
                                                                        item?.siteLogoUrl ?
                                                                            <Box className="site-info">
                                                                                <Image className="site-logo" src={item?.siteLogoUrl} />
                                                                            </Box>
                                                                            : null
                                                                    }
                                                                </Flex>
                                                            </Card.Header>
                                                            <Card.Footer className="news-footer">
                                                                <Flex>
                                                                    <Box className="read-button"><Button text content="Read" onClick={() => this.openInWindow(item?.link)} /></Box>
                                                                </Flex>
                                                            </Card.Footer>
                                                        </Card>
                                                    )
                                        }
                                    </Flex>
                                </Flex>
                            </Box>
                        </Flex.Item>
                        : null

                }
            </>
        )
    }
}

export default News;