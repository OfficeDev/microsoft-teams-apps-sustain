// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react'
import { Flex, Dialog, CloseIcon, Text, ErrorIcon } from "@fluentui/react-northstar";

class Error extends React.Component<any, any> {

    constructor(props: any) {
        super(props)

        this.state = {
            bodyMessage: props.bodyMessage,
            isOpen: props.isOpen ? props.isOpen : false
        }
    }

    dialogClose() {
        this.setState({
            isOpen: false
        })
    }

    dialogOpen() {
        this.setState({
            isOpen: true
        })
    }

    render() {
        return (
            <Flex>
                <Dialog
                    open={this.state.isOpen}
                    onOpen={() => this.dialogOpen()}
                    onCancel={() => this.dialogClose()}
                    onConfirm={() => this.dialogClose()}
                    confirmButton="Close"
                    content={<Text content={this.state.bodyMessage} />}
                    header={<ErrorIcon />}
                    headerAction={{
                        icon: <CloseIcon />,
                        title: 'Close',
                        onClick: () => this.dialogClose(),
                    }}
                />
            </Flex>
        )
    }
}

export default Error