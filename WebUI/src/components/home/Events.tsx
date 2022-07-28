// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import { Box, Card, Button, Image, Flex, Text, Skeleton } from '@fluentui/react-northstar';
import { getEvents } from 'services/sharepoint-service';
import { Event } from 'model/SharePoint/Event';

import "./Events.scss";

class Events extends React.Component<{}, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            EventsResultSet: {},
            ShowDialog: false,
            isLoading: true,
            error: false,
            selectedValueLink: ''
        }
    }

    componentDidMount() {
        this.getData();
    }

    getData() {
        this.setState({ isLoading: true }, () => {
            getEvents().then((result: any) => {
                this.setState({
                    EventsResultSet: result.data,
                    isLoading: false,
                    error: false
                }, () => console.log(this.state))

            }, err => {
                this.setState({
                    isLoading: false,
                    error: true
                })
            });
        })
    }

    openInWindow(link: string, isSeeAll: boolean) {
        if (isSeeAll) {
            if (this.state.EventsResultSet && this.state.EventsResultSet.eventsViewAllLink) {
                window.open(this.state.EventsResultSet.eventsViewAllLink)
            }
        }
        else {
            window.open(link)
        }
    }

    openDialog(link: string) {
        alert(1)
        this.setState(
            { selectedValueLink: link },  // set selected link first
            () => this.setState({ ShowDialog: true }) // set showDialog to open
        )
    }

    closeDialog() {
        this.setState({ ShowDialog: false })
    }

    render() {
        return (
            this.state.isLoading || this.state.error || (this.state.EventsResultSet.items && this.state.EventsResultSet.items.length > 0) ?
                <Flex.Item grow size="30%">
                    <Box>
                        <Flex column wrap fill id="Events" hAlign="center">
                            <Flex hAlign="center" styles={{ width: "100%" }}>
                                <Text content="Events" />
                                <Flex.Item push>
                                    <Text style={{
                                        cursor: "pointer",
                                        float: "right",
                                        color: "#5B5FC7",
                                    }}
                                        weight="semibold"
                                        content="See All"
                                        onClick={() => this.openInWindow("", true)} />
                                </Flex.Item>
                            </Flex>
                            <Flex column className="events-list">
                                {
                                    this.state.isLoading ?
                                        Array(2).fill(1, 0).map(() =>
                                            <Skeleton animation="pulse">
                                                <Card
                                                    compact
                                                    horizontal
                                                    elevated
                                                    className='event-card'
                                                    styles={{
                                                        height: '101px',
                                                        position: 'relative',
                                                        marginBottom: '15px'
                                                    }}
                                                >
                                                    <Card.Preview horizontal styles={{ height: '101px', position: 'relative' }}>
                                                        <Skeleton.Shape styles={{ height: "72px", width: "72px", marginTop: '14px', marginLeft: '14px', borderRadius: '10px' }} className='date-Box shadow' />
                                                    </Card.Preview>
                                                    <Card.Column styles={{ width: "100%", marginRight: "10px" }}>
                                                        <Card.Body>
                                                            <Flex column gap="gap.small" style={{ marginTop: '26px', marginLeft: '5px' }}>
                                                                <Skeleton.Text size="medium" style={{ width: "230px" }} />
                                                                <Skeleton.Text size="smaller" style={{ width: "230px" }} />
                                                            </Flex>
                                                        </Card.Body>
                                                    </Card.Column>
                                                </Card>
                                            </Skeleton>
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
                                            : this.state.EventsResultSet.items && this.state.EventsResultSet.items.length > 0 ?
                                                this.state.EventsResultSet.items.map((x: Event) => {
                                                    const startDate = new Date(x.start);
                                                    const endDate = new Date(x.end);
                                                    const shortMonth = startDate.toLocaleString('en-us', { 'month': 'short' });
                                                    const shortDay = startDate.toLocaleString('en-us', { 'weekday': 'short' });
                                                    const numericDay = startDate.toLocaleString('en-us', { 'day': '2-digit' });
                                                    const startTime = startDate.toLocaleString('en-us', { hour: '2-digit', minute: '2-digit' });
                                                    const endtime = endDate.toLocaleString('en-us', { hour: '2-digit', minute: '2-digit' });
                                                    const timeRange = startTime + " - " + endtime

                                                    return (
                                                        <Card
                                                            compact
                                                            horizontal
                                                            elevated
                                                            className='event-card'
                                                            styles={{
                                                                height: '101px',
                                                                position: 'relative',
                                                                marginBottom: '15px'
                                                            }}
                                                        >
                                                            <Card.Preview horizontal styles={{ height: '101px', position: 'relative' }}>
                                                                <Flex column styles={{ height: "72px", width: "72px", textAlign: "center", marginTop: '14px', marginLeft: '14px', "justify-content": "center", borderRadius: '10px' }} className='date-Box shadow'>
                                                                    <Box>
                                                                        <Text weight="semibold" content={shortMonth} color="red" styles={{ "text-transform": "uppercase" }} />
                                                                    </Box>
                                                                    <Box>
                                                                        <Text weight="bold" content={numericDay} />
                                                                    </Box>
                                                                    <Box>
                                                                        <Text content={shortDay} />
                                                                    </Box>
                                                                </Flex>
                                                            </Card.Preview>
                                                            <Card.Column className="card-column">
                                                                <Card.Body>
                                                                    <Flex column fill>
                                                                        <Text size="medium" weight="semibold" content={x.title} />
                                                                        <Text size="smaller" color="grey" content={timeRange} />
                                                                        <Flex.Item push>
                                                                            <Button text content="View" onClick={() => this.openInWindow(x?.link, false)} />
                                                                        </Flex.Item>
                                                                    </Flex>
                                                                </Card.Body>
                                                            </Card.Column>
                                                        </Card>
                                                    )
                                                })
                                                : null
                                }
                            </Flex>
                        </Flex >
                    </Box>
                </Flex.Item>
                : null
        )
    }
}

export default Events;