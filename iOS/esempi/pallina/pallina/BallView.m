//
//  BallView.m
//  pallina
//
//  Created by Davide Morelli on 27/11/2018.
//  Copyright © 2018 BioBeats. All rights reserved.
//

#import "BallView.h"
#define ballsize 50

@implementation BallView

- (void)setupWithXPosition:(float)x andYPosition:(float)y {
    self.x = x;
    self.y = y;
}

- (void)setupWithXPosition:(float)x YPosition:(float)y XVelocity:(float)dx andYVelocity:(float)dy {
    self.x = x;
    self.y = y;
    self.dx = dx;
    self.dy = dy;
    _timer = [NSTimer scheduledTimerWithTimeInterval:1.0/50.0 repeats:YES block:^(NSTimer * _Nonnull timer) {
        [self setNeedsDisplay];
    }];
}

- (void)throwIt {
    self.dy -= arc4random_uniform(100)/100.0 * 10.0;
    self.dx -= arc4random_uniform(100)/100.0 * 10.0 - 5.0;
}

// Only override drawRect: if you perform custom drawing.
// An empty implementation adversely affects performance during animation.
- (void)drawRect:(CGRect)rect {
    // Drawing code
    CGContextRef ctx = UIGraphicsGetCurrentContext();
    
    if (self.x < ballsize/2 || self.x > self.bounds.size.width - ballsize/2)
        self.dx *= -1;
    
    if (self.y < ballsize/2 || self.y > self.bounds.size.height - ballsize/2)
        self.dy *= -1;
    
    self.dx *= 0.99;
    self.dy *= 0.99;
    self.dy += 0.05;

    self.x += self.dx;
    self.y += self.dy;
    
    
    CGContextFillEllipseInRect(ctx, CGRectMake(self.x-ballsize/2, self.y-ballsize/2, ballsize, ballsize));
    NSLog(@"x=%f, y=%f", self.x, self.y);
}


@end
