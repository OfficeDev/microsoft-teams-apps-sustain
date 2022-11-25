import axios from "common/AxiosJWTDecorator";
import { globalConfig } from "config/config";
import { CarouselModel } from "model/Carousel/Carousel";


export const getCarousel = async (): Promise<any> => {

    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Carousel";

    return axios.get(url);
};

export const saveCarousel = async (carousel: CarouselModel[]): Promise<any> => {

    const formData = new FormData();
    let itemCount = 0;
    console.log(carousel)

    carousel.forEach((element, index) => {
        if (element.id === undefined && element?.fileValid !== false && element?.linkValid !== false) {
            formData.append("Link_" + itemCount, element?.link.toString());
            formData.append("Title_" + itemCount, element?.title.toString());
            formData.append("Description_" + itemCount, element?.description.toString());
            formData.append("IsActive_" + itemCount, element?.isActive.toString());

            if (element.file) {
                formData.append("ThumbnailFile_" + itemCount, element?.file);
                formData.append("ThumbnailFilename_" + itemCount, element?.file.name);
            }

            itemCount = itemCount + 1;
        }

    });

    formData.append("ItemsCount", itemCount.toString());

    if (itemCount > 0) {
        const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Carousel";

        return axios.post(url, formData);
    }
};

export const editCarousel = async (carousel: CarouselModel[]): Promise<any> => {

    const formData = new FormData();
    let itemCount = 0;
    console.log(carousel)

    carousel.forEach((element, index) => {

        if (element.id && element.id > 0 && element?.fileValid !== false && element?.linkValid !== false) {
            formData.append("Id_" + index, element?.id.toString());
            formData.append("Link_" + index, element?.link.toString());
            formData.append("Title_" + index, element?.title.toString());
            formData.append("Description_" + index, element?.description.toString());
            formData.append("IsActive_" + index, element?.isActive.toString());
            formData.append("Thumbnail_" + itemCount, element?.thumbnail.toString());

            if (element.file) {
                formData.append("ThumbnailFile_" + index, element?.file);
                formData.append("ThumbnailFilename_" + index, element?.file.name);
            }
            itemCount = itemCount + 1;
        }

    });

    formData.append("ItemsCount", itemCount.toString());

    if (itemCount > 0) {
        const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Carousel";

        return axios.put(url, formData);
    }
};


export const deleteCarousel = async (ids: number[]): Promise<any> => {
    const url = globalConfig.get().REACT_APP_BASE_URL + "/api/Carousel";

    if (ids.length > 0) {
        return axios.delete(url, { data: ids });
    }
};
