// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Card, Carousel, Flex, Skeleton, Image, Text, Box } from '@fluentui/react-northstar';
import { app } from "@microsoft/teams-js";
import { CarouselModel } from 'model/Carousel/Carousel';
import { FluentUICarousel } from 'model/Carousel/FluentUICarousel';
import { getCarousel } from 'services/carousel-service';
import "./Carousel.scss";

class CarouselComponent extends React.Component<{}, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            CarouselData: [],
            isLoading: true
        }
    }

    componentDidMount() {
        this.getCarouselData();
    }

    concatText(text: string): string {
        if (text && text.length > 70) {
            return text.substring(0, 70) + '...';
        }

        return text;
    }

    getCarouselData() {
        this.setState({
            CarouselData: [],
            isLoading: true
        }, () => {
            const carouselData: FluentUICarousel[] = [];

            getCarousel().then((res: any) => {
                res.data && res.data.items && res.data.items.length > 0 ?
                    res.data.items.forEach((x: CarouselModel) => {
                        carouselData.push({
                            "aria-label": x.id,
                            content: (
                                <Card fluid compact elevated className="carousel-card" onClick={() => this.openInWindow(x.link)}>
                                    <Card.Preview>
                                        <Image className='carousel-image' src={x.thumbnail} alt="" />
                                    </Card.Preview>
                                    <Card.Header styles={{ padding: "0 20px 5px 20px" }}>
                                        <Box styles={{ "margin-bottom": "5px" }}>
                                            <Text size='large' content={x.title} weight="semibold" />
                                        </Box>
                                        <Box>
                                            <Text content={this.concatText(x.description)} size="small" />
                                        </Box>
                                    </Card.Header>
                                </Card>
                            ),
                            id: x.id.toString(),
                            key: x.id.toString(),
                            styles: {
                                width: "100%"
                            }
                        });
                    })
                    : carouselData.push({
                        "aria-label": 0,
                        content: (
                            <Card fluid compact className='error-card carousel-card'>
                                <Card.Preview fitted className='error-preview'>
                                    <Image src="carousel-empty.svg" alt="" />
                                </Card.Preview>
                                <Card.Header className='error-text'>
                                    <Box>
                                        <Text content="Stay tuned. All you need to know about Sustainability Hub is coming soon!" size="small" />
                                    </Box>
                                </Card.Header>
                            </Card>
                        ),
                        id: "",
                        key: "",
                        styles: {
                            width: "100%"
                        }
                    });

                this.setState({
                    CarouselData: carouselData,
                    isLoading: false
                });
            }, err => {
                carouselData.push({
                    "aria-label": 0,
                    content: (
                        <Card fluid compact className='error-card carousel-card'>
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
                    ),
                    id: "",
                    key: "",
                    styles: {
                        width: "100%"
                    }
                });

                this.setState({
                    CarouselData: carouselData,
                    isLoading: false
                });
            })
        })
    }

    openInWindow(link: string) {
        var encodedWebUrl = encodeURI(link);
        app.openLink(encodedWebUrl);
    }

    render() {
        return (
            <Flex hAlign="center">
                {this.state.isLoading ?
                    <Carousel circular paddlesPosition="outside"
                        id="Carousel"
                        getItemPositionText={(index: number, size: any) => `${index + 1} of ${size}`}
                        navigation={{
                            items: this.state.CarouselData.map((item: FluentUICarousel, index: any) => ({
                                key: item.id,
                                'aria-label': item['aria-label'],
                                'aria-controls': item.id,
                            })),
                        }}
                    >
                        <Skeleton animation="pulse">
                            <Card fluid compact elevated>
                                <Card.Preview>
                                    <Skeleton.Shape className="carousel-image" as="img" />
                                </Card.Preview>
                                <Card.Header styles={{ padding: "0 20px 5px 20px" }}>
                                    <Box styles={{ "margin-bottom": "5px" }}>
                                        <Skeleton.Text size='large' styles={{ width: "50%" }} />
                                    </Box>
                                    <Box>
                                        <Skeleton.Text size="small" />
                                    </Box>
                                </Card.Header>
                            </Card>
                        </Skeleton>
                    </Carousel>
                    :
                    <Carousel circular paddlesPosition="outside"
                        id="Carousel"
                        items={this.state.CarouselData}
                        paddleNext={{ hidden: this.state.CarouselData?.length <= 1 }}
                        paddlePrevious={{ hidden: this.state.CarouselData?.length <= 1 }}
                        getItemPositionText={(index: number, size: any) => `${index + 1} of ${size}`}
                        navigation={{
                            items: this.state.CarouselData?.length > 1 ?
                                this.state.CarouselData.map((item: FluentUICarousel, index: any) => ({
                                    key: item.id,
                                    'aria-label': item['aria-label'],
                                    'aria-controls': item.id,
                                }))
                                : null
                        }}
                    />}
            </Flex>
        )
    }
}

export default CarouselComponent;