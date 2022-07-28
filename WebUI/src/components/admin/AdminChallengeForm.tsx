// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
    Form,
    FormInput,
    FormButton,
    FormRadioGroup,
    Flex,
    FormTextArea,
    FormDatepicker,
    Loader,
    Text
} from '@fluentui/react-northstar';
import { Challenge } from 'model/Challenge/Challenge';
import React from 'react';
import { addCallenge, editCallenge } from 'services/challenge-service';
import { getChallengesbyid } from 'services/learnmore-service';
import './AdminChallengeForm.scss';

interface AdminChallengeFormState {
    challenge?: Challenge;
    isLoading: boolean;
    isSaving: boolean;
    selectedFile?: File;
    errorMessage?: string;
}

class AdminChallengeForm extends React.Component<any, AdminChallengeFormState> {

    pinItems = [{ name: 'yes', key: 0, label: 'Yes', value: 1 }, { name: 'no', key: 1, label: 'No', value: 0, }];

    recurrenceItems = [{ name: 'daily', key: 0, label: 'Daily', value: 0, }, { name: 'onetime', key: 1, label: 'One-time challenge', value: 1, }];

    constructor(props: any) {
        super(props)

        if (props && props.challengeId) {
            this.state = {
                challenge: {
                    id: props.challengeId,
                    isPinned: false,
                    recurrence: 1
                },
                isLoading: true,
                isSaving: false,
                errorMessage: ''
            }
        } else {
            this.state = {
                challenge: {
                    id: 0,
                    isPinned: false,
                    recurrence: 1
                },
                isLoading: false,
                isSaving: false,
                errorMessage: ''
            }
        }

    }

    componentDidMount(): void {

        if (this.state.challenge.id > 0) {
            this.getValue(this.state.challenge.id);
        }
    }

    getValue(challengeId: any) {

        this.setState({ isLoading: true }, () => {

            getChallengesbyid(challengeId, true).then(
                (res: any) => {
                    const data = res.data;
                    if (data.items && data.items.length > 0) {
                        this.setState({
                            isLoading: false,
                            challenge: data.items[0]
                        })
                    }
                }
            )

        });

    }

    submitForm = (e: any, props: any) => {


        const title = e.target.challengeTitle.value;
        const points = e.target.pointsperday.value;
        const additionalResources = e.target.additionalResources.value;
        const focusArea = e.target.focusArea.value;

        this.setState({ isSaving: true, errorMessage: '' }, () => {
            const challenge = this.state.challenge;

            challenge.title = title;
            challenge.points = points;
            challenge.additionalResources = additionalResources;
            challenge.focusArea = focusArea;


            if (this.state.challenge.id > 0) {
                editCallenge(challenge, this.state.selectedFile).then(
                    (res: any) => {
                        this.setState({ isSaving: false })
                        this.props.closeEvent();
                        this.props.submitEvent();
                    },
                    err => {
                        this.setState({ isSaving: false, errorMessage: 'Only 4 challenges can be pinned.' });
                    }
                )
            } else {
                addCallenge(challenge, this.state.selectedFile).then(
                    (res: any) => {
                        this.setState({ isSaving: false })
                        this.props.closeEvent();
                        this.props.submitEvent();
                    },
                    err => {
                        this.setState({ isSaving: false, errorMessage: 'Only 4 challenges can be pinned.' });
                    }
                )
            }


        })

    }

    cancel() {
        if (this.props.closeEvent) {
            this.props.closeEvent();
        }
    }

    onFileChange = (event: any) => {
        this.setState({ selectedFile: event.target.files[0] });

    };

    onPinChanged = (event: any, props: any) => {
        const newChallenge = this.state.challenge;
        newChallenge.isPinned = props.value === 1;

        this.setState({ challenge: newChallenge });
    }

    onRecurrenceChanged = (event: any, props: any) => {
        const newChallenge = this.state.challenge;
        newChallenge.recurrence = props.value;

        this.setState({ challenge: newChallenge });
    }

    onActiveUntilChanged = (event: any, props: any) => {
        const date = new Date(props.value).toISOString();

        const newChallenge = this.state.challenge;
        newChallenge.activeUntil = date;

        this.setState({ challenge: newChallenge });
    }

    onFocusAreaChanged = (event: any, props: any) => {
        const newChallenge = this.state.challenge;
        newChallenge.focusArea = props.value;

        this.setState({ challenge: newChallenge });
    }

    onDescriptionChanged = (event: any, props: any) => {
        const newChallenge = this.state.challenge;
        newChallenge.description = props.value;

        this.setState({ challenge: newChallenge });
    }

    render(): React.ReactNode {
        return (
            this.state.isLoading ?
                <Loader /> :
                <Flex wrap id="admin-challenge-form">
                    <Form
                        onSubmit={this.submitForm}
                    >
                        {/* TITLE */}
                        <FormInput className="form-margin"
                            fluid
                            label="Challenge Title"
                            name="challengeTitle"
                            id="challenge-title"
                            required
                            showSuccessIndicator={false}
                            defaultValue={this.state.challenge?.title}
                        />

                        {/* IS PPINNED */}
                        <FormRadioGroup onCheckedValueChange={this.onPinChanged} id="isPinned" className="form-margin" label="Pin to dashboard" defaultCheckedValue={this.state.challenge?.isPinned ? 1 : 0} items={this.pinItems} />

                        {/* RECURRENCE */}
                        <FormRadioGroup onCheckedValueChange={this.onRecurrenceChanged} id="recurrence" className="form-margin" label="Challenge Frequency" defaultCheckedValue={this.state.challenge?.recurrence} items={this.recurrenceItems} />

                        {/* POINTS PER DAY */}
                        <FormInput className="form-margin"
                            fluid
                            label={this.state.challenge?.recurrence === 0 ? "Points per day" : "Points"}
                            name="pointsperday"
                            id="points-per-day"
                            type="number"
                            required
                            showSuccessIndicator={false}
                            defaultValue={this.state.challenge?.points?.toString()}
                        />

                        {/* THUMBNAIL */}
                        <FormInput className="form-margin"
                            fluid
                            label="Thumbnail"
                            name="thumbnail"
                            id="thumbnail"
                            type="file"
                            accept="image/*"
                            onChange={this.onFileChange}
                            showSuccessIndicator={false}
                            required={this.state.challenge.id <= 0}
                        />

                        {/* DESCRIPTION */}
                        <FormTextArea className="form-margin"
                            fluid
                            label="Description"
                            name="description"
                            id="description"
                            onChange={this.onDescriptionChanged}
                            defaultValue={this.state.challenge?.description}
                            required
                        />

                        {/* ACTIVE UNTIL */}
                        <FormDatepicker className="form-margin"
                            onDateChange={this.onActiveUntilChanged}
                            label="Active Until"
                            id="activeuntil"
                            minDate={new Date()}
                            defaultSelectedDate={this.state.challenge?.activeUntil ? new Date(this.state.challenge?.activeUntil) : new Date()}
                            required
                        />

                        {/* FOCUS AREA */}
                        {/* REASON FOR THIS COMMENTED CODE: Focus Area is subject for changes (can be a dropdown or free text)* /}
                    {/* <FormDropdown className="form-margin" onChange={this.onFocusAreaChanged} label="Focus Area" items={['Water', 'Electricity']}  defaultValue={this.state.challenge?.focusArea}/> */}
                        <FormInput className="form-margin"
                            label="Focus Area"
                            name="focusArea"
                            id="focus-rarea"
                            showSuccessIndicator={false}
                            defaultValue={this.state.challenge?.focusArea}
                            fluid
                        />

                        {/* ADDITIONAL RESOURCES */}

                        <FormInput className="form-margin"
                            label="Additional Resources"
                            name="additionalResources"
                            id="additional-resources"
                            showSuccessIndicator={false}
                            defaultValue={this.state.challenge?.additionalResources}
                            fluid
                        />

                        {
                            this.state.errorMessage ? <Text error={true} content={this.state.errorMessage} /> : null
                        }

                        {/* BUTTONS */}
                        <Flex gap="gap.small">
                            <FormButton content="Cancel" onClick={() => { this.cancel() }} />
                            <FormButton disabled={this.state.isSaving} loading={this.state.isSaving} primary content="Save" />
                        </Flex>

                    </Form>
                </Flex>
        )
    }
}

export default AdminChallengeForm;