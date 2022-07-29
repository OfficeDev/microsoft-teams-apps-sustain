// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import React from 'react';
import {
    Flex, Box, Text, Input, Image,
    Button, Form, FormInput, FormCheckbox,
    FormLabel, CheckmarkCircleIcon, FormTextArea, Loader,
    CloseIcon, Pill, TrashCanIcon
} from '@fluentui/react-northstar';
import "./ContentManagement.scss";
import { getSiteConfig, saveSiteConfig } from 'services/siteconfig-service';
import { deleteCarousel, editCarousel, getCarousel, saveCarousel } from 'services/carousel-service';
import Error from 'components/dialog/Error';
import { CarouselModel } from 'model/Carousel/Carousel';

class ContentManagement extends React.Component<any, any> {
    constructor(props: any) {
        super(props)
        this.state = {
            newsChecked: false,
            eventsChecked: false,
            sharepointUrl: '',
            sharepointUrlValid: false,
            yammerUrl: '',
            yammerUrlValid: false,
            isLoading: true,
            sharepointConfigResults: {},
            yammerConfigResults: {},
            saveAllEnable: false,
            carouselResults: {},
            editCarousel: {},
            editCarouselIndex: 0,
            carouselUrlValid: false,
            deleteCarousel: [],
            hasCarouselAdd: false,
            hasCarouselDelete: false,
        }
    }

    componentDidMount() {
        this.getSharePointSiteConfiguration(0);
        this.getYammerSiteConfiguration(1);
        this.getCarouselData();
    }

    identifyCheckBoxState = (checkBoxValue: boolean, checkBoxType: string) => {
        if (checkBoxType === "news") {
            this.setState({ newsChecked: !checkBoxValue })
        } else {
            this.setState({ eventsChecked: !checkBoxValue })
        }
    };

    getSharePointSiteConfiguration(serviceType: number) {
        this.setState({
            isLoading: true,
            sharepointConfigResults: {}
        }, () => {
            getSiteConfig(serviceType).then((res: any) => {
                this.setState({
                    sharepointConfigResults: res.data,
                    newsChecked: res.data.items[0].isNewsEnabled,
                    eventsChecked: res.data.items[0].isEventsEnabled,
                    sharepointUrl: res.data.items[0].uri,
                    sharepointUrlValid: this.validateURL(res.data.items[0].uri),
                    isLoading: false
                });
            }, err => {
                this.setState({
                    sharepointConfigResults: {},
                    isLoading: false,
                    sharepointUrlValid: false
                });
            })
        });
    }

    getYammerSiteConfiguration(serviceType: number) {
        this.setState({
            isLoading: true,
            yammerConfigResults: {},
            yammerUrl: ''
        }, () => {
            getSiteConfig(serviceType).then((res: any) => {
                this.setState({
                    yammerConfigResults: res.data,
                    yammerUrl: res.data.items[0].uri,
                    yammerUrlValid: this.validateURL(res.data.items[0].uri),
                    isLoading: false
                });
            }, err => {
                this.setState({
                    yammerConfigResults: {},
                    isLoading: false,
                    yammerUrl: '',
                    yammerUrlValid: false
                });
            })
        });
    }

    updateURLOnChange(evt: any, sourceType: string) {
        const sourceValue = evt.target.value;
        if (sourceType === "sharepoint") {
            this.setState({ sharepointUrl: sourceValue, showSharePointDialog: false })
        }
        else if (sourceType === "yammer") {
            this.setState({ yammerUrl: sourceValue, showYammerDialog: false })
        }
    }

    validateURLOnBlur(evt: any, sourceType: string, index: number) {
        const sourceValue = evt.target.value;
        const isValid = this.validateURL(sourceValue);

        if (sourceType === "sharepoint") {
            this.setState({ sharepointUrlValid: true, showSharePointDialog: false })
            if (!isValid) {
                this.setState({ sharepointUrlValid: false, showSharePointDialog: true })
            }
        }
        else if (sourceType === "yammer") {
            this.setState({ yammerUrlValid: true, showYammerDialog: false })
            if (!isValid) {
                this.setState({ yammerUrlValid: false, showYammerDialog: true })
            }
        }
        else if (sourceType === "carousel") {

            const newCarousel = this.state.editCarousel;
            newCarousel[index].linkValid = true;

            this.setState({ carouselUrlValid: true, showCarouselDialog: false, editCarousel: newCarousel })
            if (!isValid) {
                newCarousel[index].linkValid = false;
                this.setState({ carouselUrlValid: false, showCarouselDialog: true, editCarousel: newCarousel })
            }
        }
    }

    validateURL(url: string) {
        const regex = new RegExp('(https?://)?([\\da-z.-]+)\\.([a-z.]{2,6})[/\\w .-]*/?');
        return regex.test(url);
    }

    saveAll() {
        this.setState({ isLoading: true }, () => {

            this.setState({ showYammerDialog: false })
            if (this.state.yammerUrlValid) {
                this.saveYammerConfig();
            }

            this.setState({ showSharePointDialog: false })
            if (this.state.sharepointUrlValid) {
                this.saveSharePointConfig();
            }

            this.setState({ showLargeFileDialog: false, showFileTypeDialog: false, showCarouselDialog: false });
            if (this.state.hasCarouselDelete) {
                this.deleteCarouselIds();
            }

            if (this.state.hasCarouselAdd) {
                this.saveNewCarouselForm();
            }

            this.saveEditedCarouselForm();
        });
    }

    saveSharePointConfig() {
        saveSiteConfig(0, this.state.sharepointUrl, true, this.state.newsChecked, this.state.eventsChecked).then((res: any) => {
            this.getSharePointSiteConfiguration(0);
        }, err => {
            this.setState({
                isLoading: false,
                sharepointUrl: ''
            });
        })
    }

    getCarouselData() {
        this.setState({
            isLoading: true,
            carouselResults: {}
        }, () => {
            getCarousel().then((res: any) => {
                this.setState({
                    carouselResults: res.data,
                    editCarousel: res.data.items,
                    carouselUrlValid: this.validateURL(res.data.items[0]?.link),
                    isLoading: false
                });
            }, err => {
                this.setState({
                    carouselResults: {},
                    isLoading: false
                });
            })
        });
    }

    saveYammerConfig() {
        saveSiteConfig(1, this.state.yammerUrl, true, false, false).then((res: any) => {
            this.getYammerSiteConfiguration(1);
        }, err => {
            this.setState({
                isLoading: false,
                yammerUrl: ''
            });
        })
    }

    saveNewCarouselForm() {
        saveCarousel(this.state.editCarousel).then((res: any) => {
            this.setState({
                hasCarouselAdd: false
            });
            this.getCarouselData();
        }, err => {
            this.setState({
                isLoading: false,
            });
        })
    }

    saveEditedCarouselForm() {
        editCarousel(this.state.editCarousel).then((res: any) => {
            this.getCarouselData();
        }, err => {
            this.setState({
                isLoading: false,
            });
        })
    }

    deleteCarouselIds() {
        deleteCarousel(this.state.deleteCarousel).then((res: any) => {
            this.setState({
                deleteCarousel: [],
                hasCarouselDelete: false
            })
            this.getCarouselData();
        }, err => {
            this.setState({
                isLoading: false
            });
        })
    }

    onFileEdit = (event: any, index: number) => {
        const file = event.target.files[0];

        this.setState({ showLargeFileDialog: false, showFileTypeDialog: false })
        let validFile = true;

        const newCarousel = this.state.editCarousel;
        newCarousel[index].file = null;
        newCarousel[index].filename = '';
        newCarousel[index].fileValid = false;

        if (file.size > 1000000) {
            validFile = false;
            this.setState({ showLargeFileDialog: true, editCarousel: newCarousel })
        }
        else if (file.type !== 'image/jpg' && file.type !== 'image/jpeg' && file.type !== "image/png") {
            validFile = false;
            this.setState({ showFileTypeDialog: true, editCarousel: newCarousel })
        }

        if (validFile) {
            newCarousel[index].file = file;
            newCarousel[index].filename = file.name;
            newCarousel[index].fileValid = true;

            this.setState({ editCarousel: newCarousel });
        }
    };

    onEditRemoveFile(index: number) {

        const newCarousel = this.state.editCarousel;
        newCarousel[index].file = null;
        newCarousel[index].filename = ''

        this.setState({ editCarousel: newCarousel, showLargeFileDialog: false, showFileTypeDialog: false });

    };

    linkEdit(evt: any, index: number) {
        const link = evt.target.value;

        const newCarousel = this.state.editCarousel;
        newCarousel[index].link = link;

        this.setState({ editCarousel: newCarousel, showCarouselDialog: false });
    }

    titleEdit(evt: any, index: number) {
        const title = this.limitText(evt.target.value, 50);

        const newCarousel = this.state.editCarousel;
        newCarousel[index].title = title;

        this.setState({ editCarousel: newCarousel });
    }

    descriptionEdit(evt: any, index: number) {
        const description = this.limitText(evt.target.value, 255);

        const newCarousel = this.state.editCarousel;
        newCarousel[index].description = description;

        this.setState({ editCarousel: newCarousel });
    }

    editCurrentCarousel(index: number) {
        this.setState({
            editCarouselIndex: index,
            carouselUrlValid: this.validateURL(this.state.editCarousel[index]?.link)
        })
        console.log(this.state.editCarousel)
    }

    deleteCurrentCarousel(carouselItem: CarouselModel, index: number) {

        const newCarousel = this.state.editCarousel;
        newCarousel.splice(index, 1);

        this.setState({
            editCarousel: newCarousel, editCarouselIndex: 0, carouselUrlValid: this.validateURL(this.state.editCarousel[0]?.link)
        })

        if (carouselItem?.id !== undefined && carouselItem?.id > 0) {
            this.toDeleteCarousel(carouselItem.id);
        }

    }

    addNewCarousel() {
        const newCarousel = {
            link: '',
            title: '',
            thumbnail: '',
            description: '',
            isActive: true
        }

        const currentEditCarouselData = this.state.editCarousel;
        currentEditCarouselData.push(newCarousel);
        const lastEditIndex = currentEditCarouselData.length - 1;

        this.setState({
            editCarouselIndex: lastEditIndex,
            editCarousel: currentEditCarouselData,
            carouselUrlValid: false,
            hasCarouselAdd: true
        })
    }

    toDeleteCarousel(id: number) {
        const newIds = this.state.deleteCarousel;
        newIds.push(id);
        this.setState({ deleteCarousel: newIds, hasCarouselDelete: true })
    }

    limitText(text: string, characterLimit: number): string {
        if (text && text.length > characterLimit) {
            return text.substring(0, characterLimit);
        }
        return text;
    }

    render() {
        return (
            this.state.isLoading ?
                <Loader /> :
                <Flex wrap id="ContentManagement">
                    <Flex.Item grow size='40%'>
                        <Box className='content-parent'>
                            <Text weight="bold" size="larger" content='SharePoint Site' />
                            <br /><br />
                            <Text content='Add the URL of the SharePoint Site that will be providing content to this app. Indicate if you want to pull news and/or events stored in that SharePoint site collection.' />
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='40%'>
                        <Box className='content-parent'>
                            {
                                <Form className='SharePointForm'>
                                    <FormInput
                                        label="SharePoint URL"
                                        name="uri"
                                        id="uri"
                                        placeholder="Add your link here"
                                        className="LinkTextArea"
                                        value={this.state.sharepointUrl}
                                        icon={this.state.sharepointUrlValid ? <CheckmarkCircleIcon className="checkIcon" /> : <CloseIcon className="closeIcon" />}
                                        iconPosition="end"
                                        onChange={evt => this.updateURLOnChange(evt, "sharepoint")}
                                        onBlur={evt => this.validateURLOnBlur(evt, "sharepoint", 0)}
                                    />
                                    <FormLabel>Enable</FormLabel>
                                    <FormCheckbox label="News"
                                        checked={this.state.sharepointUrlValid ? this.state.newsChecked : false}
                                        disabled={this.state.sharepointUrlValid ? false : true}
                                        onClick={() => this.identifyCheckBoxState(this.state.newsChecked, "news")}
                                    />
                                    <FormCheckbox
                                        label="Events"
                                        checked={this.state.sharepointUrlValid ? this.state.eventsChecked : false}
                                        disabled={this.state.sharepointUrlValid ? false : true}
                                        onClick={() => this.identifyCheckBoxState(this.state.eventsChecked, "events")}
                                    />
                                </Form>
                            }
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='40%'>
                        <Box className='content-parent'>
                            <Text weight="bold" size="larger" content='Connect/Share' />
                            <br />
                            <br />
                            <Text content="Add the embed URL of the Yammer community that will be used to collaborate. Please use this link to generate the Yammer embedded URL: https://web.yammer.com/embed/widget" />
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='40%'>
                        <Box className='content-parent'>
                            <Form className='YammerForm'>
                                <FormInput
                                    label="Yammer embed URL"
                                    name="uri"
                                    id="uri"
                                    className="LinkTextArea"
                                    placeholder="Add your link here"
                                    value={this.state.yammerUrl}
                                    icon={this.state.yammerUrlValid ? <CheckmarkCircleIcon className="checkIcon" /> : <CloseIcon className="closeIcon" />}
                                    iconPosition="end"
                                    onChange={evt => this.updateURLOnChange(evt, "yammer")}
                                    onBlur={evt => this.validateURLOnBlur(evt, "yammer", 0)}
                                />
                            </Form>
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='30%'>
                        <Box className='content-parent'>
                            <Text weight="bold" size="larger" content='Carousel' />
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='20%'>
                        <Box className='content-parent'>
                            <Box className='ContentCarouselImport'>
                                <Text weight="bold" content="Manual Import" />
                                <Box>
                                    {
                                        this.state.editCarousel && this.state.editCarousel.length > 0 ?
                                            this.state.editCarousel.map((item: any, index: number) =>
                                                <Box>
                                                    <Button text content={"Card " + (index + 1)} onClick={() => this.editCurrentCarousel(index)} />
                                                    <Button text icon={<TrashCanIcon />} onClick={() => this.deleteCurrentCarousel(item, index)} />
                                                </Box>
                                            )
                                            : <Button text content="Add" onClick={() => this.addNewCarousel()} />
                                    }
                                    {
                                        this.state.editCarousel && (this.state.editCarousel.length > 0 && this.state.editCarousel.length < 5) ?
                                            <Button text content="Add" onClick={() => this.addNewCarousel()} />
                                            : null
                                    }
                                </Box>
                            </Box>
                        </Box>
                    </Flex.Item>
                    <Flex.Item grow size='40%'>
                        <Box className='content-parent'>
                            <Box className='ContentCarouselForm'>
                                {
                                    this.state.editCarousel && this.state.editCarousel.length > 0 ?
                                        <Form className={'CarouselForm'}>
                                            <FormInput
                                                label="Link"
                                                name="uri"
                                                id="uri"
                                                className="CarouselInput"
                                                placeholder="Add your link here"
                                                value={this.state.editCarousel[this.state.editCarouselIndex]?.link}
                                                onChange={evt => this.linkEdit(evt, this.state.editCarouselIndex)}
                                                onBlur={evt => this.validateURLOnBlur(evt, "carousel", this.state.editCarouselIndex)}
                                                icon={this.state.carouselUrlValid ? <CheckmarkCircleIcon className="checkIcon" /> : <CloseIcon className="closeIcon" />}
                                                iconPosition="end"
                                            />
                                            <FormInput
                                                label="Title"
                                                name="title"
                                                id="title"
                                                className="CarouselInput"
                                                placeholder="Add title here"
                                                onChange={evt => this.titleEdit(evt, this.state.editCarouselIndex)}
                                                value={this.state.editCarousel[this.state.editCarouselIndex]?.title}
                                            />
                                            <Box>
                                                <FormLabel>Thumbnail</FormLabel>
                                                <br />
                                                <Pill content={this.state.editCarousel[this.state.editCarouselIndex]?.filename} />
                                                <br />
                                                <Input
                                                    type="file"
                                                    accept="image/png, image/jpg, image/jpeg"
                                                    className="ThumbnailInput"
                                                    onChange={evt => this.onFileEdit(evt, this.state.editCarouselIndex)}
                                                    key={this.state.editCarousel[this.state.editCarouselIndex]?.id}
                                                >Upload</Input>
                                                <Button icon={<Image src='trash-icon.svg' />} onClick={() => this.onEditRemoveFile(this.state.editCarouselIndex)} text content="Remove" />
                                            </Box>
                                            <FormTextArea
                                                label="Description"
                                                name="description"
                                                id="description"
                                                className="CarouselTextArea"
                                                placeholder="Add your text here"
                                                onChange={evt => this.descriptionEdit(evt, this.state.editCarouselIndex)}
                                                value={this.state.editCarousel[this.state.editCarouselIndex]?.description}
                                            />
                                        </Form>
                                        : null
                                }
                            </Box>
                        </Box>
                    </Flex.Item>
                    <Flex.Item size='100%'>
                        <Box style={{ marginTop: "3%", marginBottom: "5%", marginRight: "4%" }}>
                            <Button style={{ float: "right" }} onClick={() => this.saveAll()} content="Save" primary />
                        </Box>
                    </Flex.Item>

                    {/* SharePoint Error Dialog*/}
                    {
                        !this.state.sharepointUrlValid && this.state.showSharePointDialog ?
                            <Error bodyMessage="Provided SharePoint URL is invalid. Please provide a valid link. No changes will be made to SharePoint configuration." isOpen={true} />
                            : null
                    }

                    {/* Yammer Error Dialog*/}
                    {
                        !this.state.yammerUrlValid && this.state.showYammerDialog ?
                            <Error bodyMessage="Provided Yammer URL is invalid. Please provide a valid link. No changes will be made to Yammer configuration." isOpen={true} />
                            : null
                    }

                    {/* Carousel Error Dialog*/}
                    {
                        !this.state.carouselUrlValid && this.state.showCarouselDialog ?
                            <Error bodyMessage="Provided Carousel link is invalid. Please provide a valid link. No changes will be made to Carousel configuration." isOpen={true} />
                            : null
                    }

                    {/* Invalid File Greater Than 1MB Error Dialog*/}
                    {
                        this.state.showLargeFileDialog ?
                            <Error bodyMessage="Selected file should not be more than 1 mb. No changes will be made to Carousel configuration." isOpen={true} />
                            : null
                    }

                    {/* Invalid File Greater Than 1MB Error Dialog*/}
                    {
                        this.state.showFileTypeDialog ?
                            <Error bodyMessage="Selected file type should be .png, .jpg, or .jpeg only. No changes will be made to Carousel configuration." isOpen={true} />
                            : null
                    }

                </Flex>
        )
    }
}

export default ContentManagement
