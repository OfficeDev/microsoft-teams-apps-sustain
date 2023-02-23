// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import AdminNotice from 'components/common/AdminNotice';
import React from 'react'
import { getSiteConfig } from 'services/siteconfig-service';
import "./Yammer.css";

class Yammer extends React.Component<any, any> {
    private refvar = React.createRef<HTMLIFrameElement>();
    constructor(props: any) {
        super(props)
        this.state = {
            yammerUrl: '',
            random:0
        }
    }

    componentDidMount() {
        this.getYammerSite();
    }

    resetIframe = () => {
        if(this.state.random < 1 &&  localStorage.getItem('isYammerReloadRequired') != "No")
        {
            setTimeout(() => {
                this.setState({random: this.state.random + 1});
                localStorage.setItem('isYammerReloadRequired', "No");
              }, 3000);           
        }       
      };

    getYammerSite() {
        getSiteConfig(1).then((res: any) => {
            this.setState({
                yammerConfigResults: res.data,
                yammerUrl: res.data.items[0].uri,
            }
            );
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
                <iframe name="embed-feed" title="Yammer" className='yammer' key={this.state.random} onLoad={this.resetIframe}
                src={this.state.yammerUrl} 
                />
                    
                
            </>
        );
    }
}

export default Yammer;