import React from 'react';
import { Typography, Paper, Box } from '@mui/material';
import Carousel from 'react-material-ui-carousel';
import { styled } from '@mui/system';
import image1 from '../pictures/FirstSlide.png';
import image2 from '../pictures/SecondSlide.png';
import image3 from '../pictures/ThirdSlide.png';

const StyledCarousel = styled(Carousel)(({ theme }) => ({
    margin: 'auto',
    marginTop: theme.spacing(5),
    width: '80%',
    overflow: 'hidden',
    borderRadius: theme.shape.borderRadius,
}));

const CarouselItem = styled(Paper)(({ theme }) => ({
    padding: theme.spacing(5),
    textAlign: 'center',
    color: theme.palette.text.secondary,
    // Set a minimum height that fits your largest image
    minHeight: '500px', // Adjust this value based on your actual image sizes
    lineHeight: '1', // Remove fixed line height to allow content to determine the height
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center', // This centers the content vertically
    alignItems: 'center', // This centers the content horizontally
}));

export function Home() {
    const slides = [
        {
            label: "Register",
            description: "Create your account.",
            imageUrl: image2
        },
        {
            label: "Login",
            description: "Log in with your account.",
            imageUrl: image1

        },
        {
            label: "Welcome aboard!",
            description: "Enjoy your own personal board.",
            imageUrl: image3

        }
    ];

    return (
        <Box>
            <StyledCarousel autoPlay={true} interval={3000}>
                {slides.map((slide, index) => (
                    <CarouselItem key={index} elevation={4}>
                        <Typography variant="h4">{slide.label}</Typography>
                        <Typography>{slide.description}</Typography>
                        <img src={slide.imageUrl} alt={slide.label} style={{ width: '100%', height: 'auto' }} />
                    </CarouselItem>
                ))}
            </StyledCarousel>
        </Box>
    );
}
