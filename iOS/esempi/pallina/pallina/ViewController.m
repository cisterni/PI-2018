//
//  ViewController.m
//  pallina
//
//  Created by Davide Morelli on 27/11/2018.
//  Copyright © 2018 BioBeats. All rights reserved.
//

#import "ViewController.h"
#import "BallView.h"

@interface ViewController ()
@property (weak, nonatomic) IBOutlet BallView *myBallView;

@end

@implementation ViewController

- (IBAction)tiraLaPalla:(id)sender {
    [self.myBallView throwIt];
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
    //[self.myBallView setupWithXPosition:100.0 andYPosition:100.0];
    [self.myBallView setupWithXPosition:100.0 YPosition:100.0 XVelocity:arc4random_uniform(100)/100.0*-10.0 andYVelocity:arc4random_uniform(100)/100.0*10.0-5.0];
}


@end
