import { Box, Button, ChevronStartIcon, Flex, List, Segment, Text } from "@fluentui/react-northstar";
import AdminChallenge from "components/admin/AdminChallenge";
import React from 'react';
import AdminUserManagement from "components/admin/AdminUserManagement";
import ContentManagement from "components/admin/ContentManagement";
import { pages } from "@microsoft/teams-js";
import { globalConfig } from "config/config";

class Admin extends React.Component<{}, any> {

    items: any[] = [
        {
            key: 'contentManagement',
            header: 'Content Management',
            headerMedia: '',
            content: ''
        },
        {
            key: 'adminGroups',
            header: 'Admins & Groups',
            headerMedia: '',
            content: ''
        },
        {
            key: 'challenges',
            header: 'Challenges',
            headerMedia: '',
            content: ''
        }
    ]

    constructor(props: any) {
        super(props);
        this.state = {
            selectedTab: 'contentManagement'
        }
    }

    componentDidMount(): void {

    }

    handleChange = (event: any, props: any) => {
        const selectedTab = this.items[props.selectedIndex].key;
        this.setState({ selectedTab: selectedTab });
    }

    back() {
        pages.navigateToApp({ appId: globalConfig.get().REACT_APP_TEAMS_APP_ID, pageId: globalConfig.get().REACT_APP_HOME_PAGE_ID })
    }

    render() {
        return (
            <>
                <Segment className="navigation" styles={{ width: "100%" }} >
                    <Button className="back" icon={<ChevronStartIcon />} onClick={() => this.back()} content="Back to Home" text />
                </Segment>
                <Flex id="adminHome">
                    <Flex.Item size="20%">
                        <Box className="list-container">
                            <List
                                selectable
                                defaultSelectedIndex={0}
                                items={this.items}
                                onSelectedIndexChange={this.handleChange}
                            />

                        </Box>
                    </Flex.Item>
                    <Flex.Item size="80%">
                        <Box>
                            {
                                this.state?.selectedTab == 'contentManagement' ? <ContentManagement /> : null
                            }
                            {
                                this.state?.selectedTab == 'adminGroups' ? <AdminUserManagement /> : null
                            }
                            {
                                this.state?.selectedTab == 'challenges' ? <AdminChallenge /> : null
                            }
                        </Box>
                    </Flex.Item>
                </Flex>
            </>
        )
    }
}

export default Admin;