// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import AdminNotice from 'components/common/AdminNotice';
import React from 'react'
import { getSiteConfig } from 'services/siteconfig-service';
import "./Yammer.css";

class Yammer extends React.Component<any, any> {
    constructor(props: any) {
        super(props)

        this.state = {
            yammerUrl: ''
        }
    }

    componentDidMount() {
        this.getYammerSite();
    }

    getYammerSite() {
        getSiteConfig(1).then((res: any) => {
            this.setState({
                yammerConfigResults: res.data,
                yammerUrl: res.data.items[0].uri,
            });
        }, err => {
            this.setState({
                yammerUrl: ''
            });
        })
    }

    render() {
        return (
            <>
                <AdminNotice />
                <iframe name='embed-feed' title='Yammer' className='yammer' src={this.state.yammerUrl} />
            </>
        );
    }
}

export default Yammer;